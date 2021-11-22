/*++

    Copyright (c) Microsoft Corporation.
    Licensed under the MIT License.

--*/

#include "wkt.h"

typedef enum _SYSTEM_INFORMATION_CLASS {
    SystemBasicInformation                          = 0
} SYSTEM_INFORMATION_CLASS;

NTSYSAPI // Copied from zwapi.h.
NTSTATUS
NTAPI
ZwQuerySystemInformation (
    __in SYSTEM_INFORMATION_CLASS SystemInformationClass,
    __out_bcount_part_opt(SystemInformationLength, *ReturnLength) PVOID SystemInformation,
    __in ULONG SystemInformationLength,
    __out_opt PULONG ReturnLength
    );

typedef struct _SYSTEM_BASIC_INFORMATION {
    ULONG Reserved;
    ULONG TimerResolution;
    ULONG PageSize;

    //
    // WARNING: The following fields are 32-bit and may get
    // capped to MAXULONG on systems with a lot of RAM!
    //
    // Use SYSTEM_PHYSICAL_MEMORY_INFORMATION instead.
    //

    ULONG NumberOfPhysicalPages;      // Deprecated, do not use.
    ULONG LowestPhysicalPageNumber;   // Deprecated, do not use.
    ULONG HighestPhysicalPageNumber;  // Deprecated, do not use.

    ULONG AllocationGranularity;
    ULONG_PTR MinimumUserModeAddress;
    ULONG_PTR MaximumUserModeAddress;
    ULONG_PTR ActiveProcessorsAffinityMask;
    CCHAR NumberOfProcessors;
} SYSTEM_BASIC_INFORMATION, *PSYSTEM_BASIC_INFORMATION;

typedef struct WKT_PLATFORM {

    PDRIVER_OBJECT DriverObject;

    //
    // Random number algorithm loaded for DISPATCH_LEVEL usage.
    //
    BCRYPT_ALG_HANDLE RngAlgorithm;

} WKT_PLATFORM;

uint64_t PlatformPerfFreq;
uint64_t WtkTotalMemory;
WKT_PLATFORM Platform = { NULL, NULL };

INITCODE
_IRQL_requires_max_(PASSIVE_LEVEL)
void
PlatformSystemLoad(
    _In_ PDRIVER_OBJECT DriverObject,
    _In_ PUNICODE_STRING RegistryPath
    )
{
    UNREFERENCED_PARAMETER(RegistryPath);

    Platform.DriverObject = DriverObject;
    (VOID)KeQueryPerformanceCounter((LARGE_INTEGER*)&PlatformPerfFreq);
    Platform.RngAlgorithm = NULL;
}

PAGEDX
_IRQL_requires_max_(PASSIVE_LEVEL)
void
PlatformSystemUnload(
    void
    )
{
    PAGED_CODE();
}

PAGEDX
_IRQL_requires_max_(PASSIVE_LEVEL)
NTSTATUS
PlatformInitialize(
    void
    )
{
    SYSTEM_BASIC_INFORMATION Sbi;

    PAGED_CODE();

    NTSTATUS Status =
        BCryptOpenAlgorithmProvider(
            &Platform.RngAlgorithm,
            BCRYPT_RNG_ALGORITHM,
            NULL,
            BCRYPT_PROV_DISPATCH);
    if (!NT_SUCCESS(Status)) {
        goto Error;
    }
    WKT_DBG_ASSERT(Platform.RngAlgorithm != NULL);

    Status =
        ZwQuerySystemInformation(
            SystemBasicInformation, &Sbi, sizeof(Sbi), NULL);
    if (!NT_SUCCESS(Status)) {
        goto Error;
    }

    WtkTotalMemory = (uint64_t)Sbi.NumberOfPhysicalPages * (uint64_t)Sbi.PageSize;

Error:

    if (!NT_SUCCESS(Status)) {
        if (Platform.RngAlgorithm != NULL) {
            BCryptCloseAlgorithmProvider(Platform.RngAlgorithm, 0);
            Platform.RngAlgorithm = NULL;
        }
    }

    return Status;
}

PAGEDX
_IRQL_requires_max_(PASSIVE_LEVEL)
void
PlatformUninitialize(
    void
    )
{
    PAGED_CODE();
    BCryptCloseAlgorithmProvider(Platform.RngAlgorithm, 0);
    Platform.RngAlgorithm = NULL;
}

_IRQL_requires_max_(DISPATCH_LEVEL)
void
PlatformLogAssert(
    _In_z_ const char* File,
    _In_ int Line,
    _In_z_ const char* Expr
    )
{
    UNREFERENCED_PARAMETER(File);
    UNREFERENCED_PARAMETER(Line);
    UNREFERENCED_PARAMETER(Expr);
}

_IRQL_requires_max_(DISPATCH_LEVEL)
NTSTATUS
WtkRandom(
    _In_ uint32_t BufferLen,
    _Out_writes_bytes_(BufferLen) void* Buffer
    )
{
    //
    // Use the algorithm we initialized for DISPATCH_LEVEL usage.
    //
    WKT_DBG_ASSERT(Platform.RngAlgorithm != NULL);
    return (NTSTATUS)
        BCryptGenRandom(
            Platform.RngAlgorithm,
            (uint8_t*)Buffer,
            BufferLen,
            0);
}
