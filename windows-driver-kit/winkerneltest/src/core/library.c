/*++

    Copyright (c) Microsoft Corporation.
    Licensed under the MIT License.

Abstract:

    General library functions

--*/

#include "wkt.h"

typedef struct WKT_LIBRARY {

    BOOLEAN Loaded;
    WKT_LOCK Lock;
    WKT_DISPATCH_LOCK DatapathLock;
    uint32_t RefCount;

} WKT_LIBRARY;

WKT_LIBRARY WktLib = { 0 };

INITCODE
_IRQL_requires_max_(PASSIVE_LEVEL)
void
CoreLibraryLoad(
    void
    )
{
    WktLockInitialize(&WktLib.Lock);
    WktDispatchLockInitialize(&WktLib.DatapathLock);
    WktLib.Loaded = TRUE;
}

//
// Uninitializes global variables.
//
_IRQL_requires_max_(PASSIVE_LEVEL)
void
CoreLibraryUnload(
    void
    )
{
    WKT_FRE_ASSERT(WktLib.Loaded);
    WktLib.Loaded = FALSE;
    WktDispatchLockUninitialize(&WktLib.DatapathLock);
    WktLockUninitialize(&WktLib.Lock);
}

_IRQL_requires_max_(PASSIVE_LEVEL)
NTSTATUS
CoreLibraryInitialize(
    void
    )
{
    return PlatformInitialize();
}

_IRQL_requires_max_(PASSIVE_LEVEL)
void
CoreLibraryUninitialize(
    void
    )
{
    PlatformUninitialize();
}

_IRQL_requires_max_(PASSIVE_LEVEL)
NTSTATUS
CoreAddRef(
    void
    )
{
    //
    // If you hit this assert, you are trying to call Core API without
    // actually loading/starting the library/driver.
    //
    WKT_TEL_ASSERT(WktLib.Loaded);
    if (!WktLib.Loaded) {
        return STATUS_INVALID_DISPOSITION;
    }

    NTSTATUS Status = STATUS_SUCCESS;

    WktLockAcquire(&WktLib.Lock);

    //
    // Increment global ref count, and if this is the first ref, initialize all
    // the global library state.
    //
    if (++WktLib.RefCount == 1) {
        Status = CoreLibraryInitialize();
        if (!NT_SUCCESS(Status)) {
            WktLib.RefCount--;
            goto Error;
        }
    }

Error:

    WktLockRelease(&WktLib.Lock);

    return Status;
}

_IRQL_requires_max_(PASSIVE_LEVEL)
void
CoreRelease(
    void
    )
{
    WktLockAcquire(&WktLib.Lock);

    //
    // Decrement global ref count and uninitialize the library if this is the
    // last ref.
    //

    WKT_FRE_ASSERT(WktLib.RefCount > 0);

    if (--WktLib.RefCount == 0) {
        CoreLibraryUninitialize();
    }

    WktLockRelease(&WktLib.Lock);
}

_IRQL_requires_max_(PASSIVE_LEVEL)
NTSTATUS
WKT_API
WinKernelTestOpen(
    _Out_ _Pre_defensive_ const void** WktApi
    )
{
    NTSTATUS Status;

    if (WktApi == NULL) {
        Status = STATUS_INVALID_PARAMETER;;
        goto Exit;
    }

    Status = CoreAddRef();
    if (!NT_SUCCESS(Status)) {
        goto Exit;
    }

    void** Api = WKT_ALLOC_NONPAGED(sizeof(void*));
    if (Api == NULL) {
        Status = STATUS_NO_MEMORY;
        goto Error;
    }

    *WktApi = Api;

Error:

    if (!NT_SUCCESS(Status)) {
        CoreRelease();
    }

Exit:

    return Status;
}

_IRQL_requires_max_(PASSIVE_LEVEL)
void
WKT_API
WinKernelTestClose(
    _In_ _Pre_defensive_ const void* WktApi
    )
{
    if (WktApi != NULL) {
        WKT_FREE(WktApi);
        CoreRelease();
    }
}
