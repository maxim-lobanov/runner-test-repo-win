/*++

    Copyright (c) Microsoft Corporation.
    Licensed under the MIT License.

Abstract:

    Platform definitions.

--*/

#pragma once

#pragma warning(push) // Don't care about OACR warnings in publics
#pragma warning(disable:26036)
#pragma warning(disable:26061)
#pragma warning(disable:26071)
#pragma warning(disable:28118)
#pragma warning(disable:28196)
#pragma warning(disable:28252)
#pragma warning(disable:28253)
#pragma warning(disable:28309)
#include <ntifs.h>
#include <ntverp.h>
#include <ntstrsafe.h>
#include <WppRecorder.h>
#include <wdf.h>
#include <netioapi.h>
#include <wsk.h>
#include <bcrypt.h>
#include <intrin.h>
#include <ws2def.h>
#include <ws2ipdef.h>
#include <minwindef.h>
#include <ntstatus.h>
#include <basetsd.h>
#pragma warning(pop)

typedef INT8 int8_t;
typedef INT16 int16_t;
typedef INT32 int32_t;
typedef INT64 int64_t;

typedef UINT8 uint8_t;
typedef UINT16 uint16_t;
typedef UINT32 uint32_t;
typedef UINT64 uint64_t;

#define UINT8_MAX   0xffui8
#define UINT16_MAX  0xffffui16
#define UINT32_MAX  0xffffffffui32
#define UINT64_MAX  0xffffffffffffffffui64

#define NS_TO_US(x)     ((x) / 1000)
#define US_TO_NS(x)     ((x) * 1000)
#define NS100_TO_US(x)  ((x) / 10)
#define US_TO_NS100(x)  ((x) * 10)
#define MS_TO_NS100(x)  ((x)*10000)
#define NS100_TO_MS(x)  ((x)/10000)
#define US_TO_MS(x)     ((x) / 1000)
#define MS_TO_US(x)     ((x) * 1000)
#define US_TO_S(x)      ((x) / (1000 * 1000))
#define S_TO_US(x)      ((x) * 1000 * 1000)
#define S_TO_NS(x)      ((x) * 1000 * 1000 * 1000)
#define MS_TO_S(x)      ((x) / 1000)
#define S_TO_MS(x)      ((x) * 1000)

#if (NTDDI_VERSION >= NTDDI_WIN2K) // Copied from zwapi_x.h.
_IRQL_requires_max_(PASSIVE_LEVEL)
NTSYSAPI
NTSTATUS
NTAPI
ZwSetInformationThread(
    _In_ HANDLE ThreadHandle,
    _In_ THREADINFOCLASS ThreadInformationClass,
    _In_reads_bytes_(ThreadInformationLength) PVOID ThreadInformation,
    _In_ ULONG ThreadInformationLength
    );
#endif

#define WKT_API NTAPI

#if _WIN64
#define WKT_64BIT 1
#else
#define WKT_32BIT 1
#endif

#ifndef KRTL_INIT_SEGMENT
#define KRTL_INIT_SEGMENT "INIT"
#endif
#ifndef KRTL_PAGE_SEGMENT
#define KRTL_PAGE_SEGMENT "PAGE"
#endif
#ifndef KRTL_NONPAGED_SEGMENT
#define KRTL_NONPAGED_SEGMENT ".text"
#endif

// Use on code in the INIT segment. (Code is discarded after DriverEntry returns.)
#define INITCODE __declspec(code_seg(KRTL_INIT_SEGMENT))

// Use on pageable functions.
#define PAGEDX __declspec(code_seg(KRTL_PAGE_SEGMENT))

#define WKT_CACHEALIGN DECLSPEC_CACHEALIGN

//
// Library Initialization
//

//
// Called in DLLMain or DriverEntry.
//
INITCODE
_IRQL_requires_max_(PASSIVE_LEVEL)
void
PlatformSystemLoad(
    _In_ PDRIVER_OBJECT DriverObject,
    _In_ PUNICODE_STRING RegistryPath
    );

//
// Called in DLLMain or DriverUnload.
//
PAGEDX
_IRQL_requires_max_(PASSIVE_LEVEL)
void
PlatformSystemUnload(
    void
    );

//
// Initializes the PAL library. Calls to this and
// PlatformUninitialize must be serialized and cannot overlap.
//
PAGEDX
_IRQL_requires_max_(PASSIVE_LEVEL)
NTSTATUS
PlatformInitialize(
    void
    );

//
// Uninitializes the PAL library. Calls to this and
// PlatformInitialize must be serialized and cannot overlap.
//
PAGEDX
_IRQL_requires_max_(PASSIVE_LEVEL)
void
PlatformUninitialize(
    void
    );

//
// Assertion Interfaces
//

#define WKT_STATIC_ASSERT(X,Y) static_assert(X,Y)

#define WKT_ANALYSIS_ASSERT(X) __analysis_assert(X)

//
// Logs the assertion failure to ETW.
//
_IRQL_requires_max_(DISPATCH_LEVEL)
void
PlatformLogAssert(
    _In_z_ const char* File,
    _In_ int Line,
    _In_z_ const char* Expr
    );

#define WKT_ASSERT_ACTION(_exp) \
    ((!(_exp)) ? \
        (PlatformLogAssert(__FILE__, __LINE__, #_exp), \
         __annotation(L"Debug", L"AssertFail", L#_exp), \
         DbgRaiseAssertionFailure(), FALSE) : \
        TRUE)

#define WKT_ASSERTMSG_ACTION(_msg, _exp) \
    ((!(_exp)) ? \
        (PlatformLogAssert(__FILE__, __LINE__, #_exp), \
         __annotation(L"Debug", L"AssertFail", L##_msg), \
         DbgRaiseAssertionFailure(), FALSE) : \
        TRUE)

#if defined(_PREFAST_)
// _Analysis_assume_ will never result in any code generation for _exp,
// so using it will not have runtime impact, even if _exp has side effects.
#define WKT_ANALYSIS_ASSUME(_exp) _Analysis_assume_(_exp)
#else // _PREFAST_
// WKT_ANALYSIS_ASSUME ensures that _exp is parsed in non-analysis compile.
// On DBG, it's guaranteed to be parsed as part of the normal compile, but with
// non-DBG, use __noop to ensure _exp is parseable but without code generation.
#if DBG
#define WKT_ANALYSIS_ASSUME(_exp) ((void) 0)
#else // DBG
#define WKT_ANALYSIS_ASSUME(_exp) __noop(_exp)
#endif // DBG
#endif // _PREFAST_

//
// MsWkt uses three types of asserts:
//
//  WKT_DBG_ASSERT - Asserts that are too expensive to evaluate all the time.
//  WKT_TEL_ASSERT - Asserts that are acceptable to always evaluate, but not
//                    always crash the system.
//  WKT_FRE_ASSERT - Asserts that must always crash the system.
//

#if DBG || WKT_TEST_MODE
#define WKT_DBG_ASSERT(_exp)          (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERT_ACTION(_exp))
#define WKT_DBG_ASSERTMSG(_exp, _msg) (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERTMSG_ACTION(_msg, _exp))
#else
#define WKT_DBG_ASSERT(_exp)          (WKT_ANALYSIS_ASSUME(_exp), 0)
#define WKT_DBG_ASSERTMSG(_exp, _msg) (WKT_ANALYSIS_ASSUME(_exp), 0)
#endif

#if DBG || WKT_TEST_MODE
#define WKT_TEL_ASSERT(_exp)          (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERT_ACTION(_exp))
#define WKT_TEL_ASSERTMSG(_exp, _msg) (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERTMSG_ACTION(_msg, _exp))
#define WKT_TEL_ASSERTMSG_ARGS(_exp, _msg, _origin, _bucketArg1, _bucketArg2) \
     (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERTMSG_ACTION(_msg, _exp))
#else
#ifdef MICROSOFT_TELEMETRY_ASSERT
#define WKT_TEL_ASSERT(_exp)          (WKT_ANALYSIS_ASSUME(_exp), MICROSOFT_TELEMETRY_ASSERT_KM(_exp))
#define WKT_TEL_ASSERTMSG(_exp, _msg) (WKT_ANALYSIS_ASSUME(_exp), MICROSOFT_TELEMETRY_ASSERT_MSG_KM(_exp, _msg))
#define WKT_TEL_ASSERTMSG_ARGS(_exp, _msg, _origin, _bucketArg1, _bucketArg2) \
    (WKT_ANALYSIS_ASSUME(_exp), MICROSOFT_TELEMETRY_ASSERT_MSG_WITH_ARGS_KM(_exp, _msg, _origin, _bucketArg1, _bucketArg2))
#else
#define WKT_TEL_ASSERT(_exp)          (WKT_ANALYSIS_ASSUME(_exp), 0)
#define WKT_TEL_ASSERTMSG(_exp, _msg) (WKT_ANALYSIS_ASSUME(_exp), 0)
#define WKT_TEL_ASSERTMSG_ARGS(_exp, _msg, _origin, _bucketArg1, _bucketArg2) \
    (WKT_ANALYSIS_ASSUME(_exp), 0)
#endif
#endif

#define WKT_FRE_ASSERT(_exp)          (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERT_ACTION(_exp))
#define WKT_FRE_ASSERTMSG(_exp, _msg) (WKT_ANALYSIS_ASSUME(_exp), WKT_ASSERTMSG_ACTION(_msg, _exp))

//
// Verifier is enabled.
//
#define WktVerifierEnabled(Flags) NT_SUCCESS(MmIsVerifierEnabled((PULONG)&Flags))
#define WktVerifierEnabledByAddr(Address) MmIsDriverVerifyingByAddress(Address)

//
// Debugger check.
//
#define WktDebuggerPresent() KD_DEBUGGER_ENABLED

//
// Interrupt ReQuest Level
//

#define WKT_IRQL() KeGetCurrentIrql()

#define WKT_PASSIVE_CODE() WKT_DBG_ASSERT(WKT_IRQL() == PASSIVE_LEVEL)

//
// Allocation/Memory Interfaces
//

extern uint64_t WktTotalMemory;

#define WKT_POOL_TAG '.TKW'

#define WKT_ALLOC_PAGED(Size) ExAllocatePoolWithTag(PagedPool, Size, WKT_POOL_TAG)
#define WKT_ALLOC_NONPAGED(Size) ExAllocatePoolWithTag(NonPagedPoolNx, Size, WKT_POOL_TAG)
#define WKT_FREE(Mem) ExFreePoolWithTag((void*)Mem, WKT_POOL_TAG)

typedef LOOKASIDE_LIST_EX WKT_POOL;

#define WktPoolInitialize(IsPaged, Size, Pool) \
    ExInitializeLookasideListEx( \
        Pool, \
        NULL, \
        NULL, \
        (IsPaged) ? PagedPool : NonPagedPoolNx, \
        0, \
        Size, \
        WKT_POOL_TAG, \
        0)

#define WktPoolUninitialize(Pool) ExDeleteLookasideListEx(Pool)
#define WktPoolAlloc(Pool) ExAllocateFromLookasideListEx(Pool)
#define WktPoolFree(Pool, Entry) ExFreeToLookasideListEx(Pool, Entry)

#define WktZeroMemory RtlZeroMemory
#define WktCopyMemory RtlCopyMemory
#define WktMoveMemory RtlMoveMemory
#define WktSecureZeroMemory RtlSecureZeroMemory

#define WktByteSwapUint16 RtlUshortByteSwap
#define WktByteSwapUint32 RtlUlongByteSwap
#define WktByteSwapUint64 RtlUlonglongByteSwap

//
// Locking Interfaces
//

//
// The following declares several currently unpublished shared locking
// functions from Windows.
//

__drv_maxIRQL(APC_LEVEL)
__drv_mustHoldCriticalRegion
NTKERNELAPI
VOID
FASTCALL
ExfAcquirePushLockExclusive(
    __inout __deref __drv_acquiresExclusiveResource(ExPushLockType)
    PEX_PUSH_LOCK PushLock
    );

__drv_maxIRQL(APC_LEVEL)
__drv_mustHoldCriticalRegion
NTKERNELAPI
VOID
FASTCALL
ExfAcquirePushLockShared(
    __inout __deref __drv_acquiresExclusiveResource(ExPushLockType)
    PEX_PUSH_LOCK PushLock
    );

__drv_maxIRQL(DISPATCH_LEVEL)
__drv_mustHoldCriticalRegion
NTKERNELAPI
VOID
FASTCALL
ExfReleasePushLockExclusive(
    __inout __deref __drv_releasesExclusiveResource(ExPushLockType)
    PEX_PUSH_LOCK PushLock
    );

__drv_maxIRQL(DISPATCH_LEVEL)
__drv_mustHoldCriticalRegion
NTKERNELAPI
VOID
FASTCALL
ExfReleasePushLockShared(
    __inout __deref __drv_releasesExclusiveResource(ExPushLockType)
    PEX_PUSH_LOCK PushLock
    );

typedef EX_PUSH_LOCK WKT_LOCK;

#define WktLockInitialize(Lock) ExInitializePushLock(Lock)
#define WktLockUninitialize(Lock)
#define WktLockAcquire(Lock) KeEnterCriticalRegion(); ExfAcquirePushLockExclusive(Lock)
#define WktLockRelease(Lock) ExfReleasePushLockExclusive(Lock); KeLeaveCriticalRegion()

typedef struct WKT_DISPATCH_LOCK {
    KSPIN_LOCK SpinLock;
    KIRQL PrevIrql;
} WKT_DISPATCH_LOCK;

#define WktDispatchLockInitialize(Lock) KeInitializeSpinLock(&(Lock)->SpinLock)
#define WktDispatchLockUninitialize(Lock)
#if defined(_AMD64_) || defined(_ARM64_)
#define WktDispatchLockAcquire(Lock) (Lock)->PrevIrql = KeAcquireSpinLockRaiseToDpc(&(Lock)->SpinLock)
#else
#define WktDispatchLockAcquire(Lock) KeAcquireSpinLock(&(Lock)->SpinLock, &(Lock)->PrevIrql)
#endif
#define WktDispatchLockRelease(Lock) KeReleaseSpinLock(&(Lock)->SpinLock, (Lock)->PrevIrql)

typedef EX_PUSH_LOCK WKT_RW_LOCK;

#define WktRwLockInitialize(Lock) ExInitializePushLock(Lock)
#define WktRwLockUninitialize(Lock)
#define WktRwLockAcquireShared(Lock) KeEnterCriticalRegion(); ExfAcquirePushLockShared(Lock)
#define WktRwLockAcquireExclusive(Lock) KeEnterCriticalRegion(); ExfAcquirePushLockExclusive(Lock)
#define WktRwLockReleaseShared(Lock) ExfReleasePushLockShared(Lock); KeLeaveCriticalRegion()
#define WktRwLockReleaseExclusive(Lock) ExfReleasePushLockExclusive(Lock); KeLeaveCriticalRegion()

typedef struct WKT_DISPATCH_RW_LOCK {
    EX_SPIN_LOCK SpinLock;
    KIRQL PrevIrql;
} WKT_DISPATCH_RW_LOCK;

#define WktDispatchRwLockInitialize(Lock) (Lock)->SpinLock = 0
#define WktDispatchRwLockUninitialize(Lock)
#define WktDispatchRwLockAcquireShared(Lock) (Lock)->PrevIrql = ExAcquireSpinLockShared(&(Lock)->SpinLock)
#define WktDispatchRwLockAcquireExclusive(Lock) (Lock)->PrevIrql = ExAcquireSpinLockExclusive(&(Lock)->SpinLock)
#define WktDispatchRwLockReleaseShared(Lock) ExReleaseSpinLockShared(&(Lock)->SpinLock, (Lock)->PrevIrql)
#define WktDispatchRwLockReleaseExclusive(Lock) ExReleaseSpinLockExclusive(&(Lock)->SpinLock, (Lock)->PrevIrql)

//
// Reference Count Interface
//

#if defined(_X86_) || defined(_AMD64_)
#define WktBarrierAfterInterlock()
#elif defined(_ARM64_)
#define WktBarrierAfterInterlock()  __dmb(_ARM64_BARRIER_ISH)
#elif defined(_ARM_)
#define WktBarrierAfterInterlock()  __dmb(_ARM_BARRIER_ISH)
#else
#error Unsupported architecture.
#endif

#if defined (_WIN64)
#define WktIncrementLongPtrNoFence InterlockedIncrementNoFence64
#define WktDecrementLongPtrRelease InterlockedDecrementRelease64
#define WktCompareExchangeLongPtrNoFence InterlockedCompareExchangeNoFence64
#define WktReadLongPtrNoFence ReadNoFence64
#else
#define WktIncrementLongPtrNoFence InterlockedIncrementNoFence
#define WktDecrementLongPtrRelease InterlockedDecrementRelease
#define WktCompareExchangeLongPtrNoFence InterlockedCompareExchangeNoFence
#define WktReadLongPtrNoFence ReadNoFence
#endif

typedef LONG_PTR WKT_REF_COUNT;

inline
void
WktRefInitialize(
    _Out_ WKT_REF_COUNT* RefCount
    )
{
    *RefCount = 1;
}

#define WktRefUninitialize(RefCount)

inline
void
WktRefIncrement(
    _Inout_ WKT_REF_COUNT* RefCount
    )
{
    if (WktIncrementLongPtrNoFence(RefCount) > 1) {
        return;
    }

    __fastfail(FAST_FAIL_INVALID_REFERENCE_COUNT);
}

inline
BOOLEAN
WktRefIncrementNonZero(
    _Inout_ volatile WKT_REF_COUNT *RefCount,
    _In_ ULONG Bias
    )
{
    WKT_REF_COUNT NewValue;
    WKT_REF_COUNT OldValue;

    PrefetchForWrite(RefCount);
    OldValue = WktReadLongPtrNoFence(RefCount);
    for (;;) {
        NewValue = OldValue + Bias;
        if ((ULONG_PTR)NewValue > Bias) {
            NewValue = WktCompareExchangeLongPtrNoFence(RefCount,
                                                         NewValue,
                                                         OldValue);
            if (NewValue == OldValue) {
                return TRUE;
            }

            OldValue = NewValue;

        } else if ((ULONG_PTR)NewValue == Bias) {
            return FALSE;

        } else {
            __fastfail(FAST_FAIL_INVALID_REFERENCE_COUNT);
            return FALSE;
        }
    }
}

inline
BOOLEAN
WktRefDecrement(
    _Inout_ WKT_REF_COUNT* RefCount
    )
{
    WKT_REF_COUNT NewValue;

    //
    // A release fence is required to ensure all guarded memory accesses are
    // complete before any thread can begin destroying the object.
    //

    NewValue = WktDecrementLongPtrRelease(RefCount);
    if (NewValue > 0) {
        return FALSE;

    } else if (NewValue == 0) {

        //
        // An acquire fence is required before object destruction to ensure
        // that the destructor cannot observe values changing on other threads.
        //

        WktBarrierAfterInterlock();
        return TRUE;
    }

    __fastfail(FAST_FAIL_INVALID_REFERENCE_COUNT);
    return FALSE;
}

//
// Event Interfaces
//

typedef KEVENT WKT_EVENT;
#define WktEventInitialize(Event, ManualReset, InitialState) \
    KeInitializeEvent(Event, ManualReset ? NotificationEvent : SynchronizationEvent, InitialState)
#define WktEventUninitialize(Event)
#define WktEventSet(Event) KeSetEvent(&(Event), IO_NO_INCREMENT, FALSE)
#define WktEventReset(Event) KeResetEvent(&(Event))
#define WktEventWaitForever(Event) \
    KeWaitForSingleObject(&(Event), Executive, KernelMode, FALSE, NULL)
inline
NTSTATUS
_WktEventWaitWithTimeout(
    _In_ WKT_EVENT* Event,
    _In_ uint32_t TimeoutMs
    )
{
    LARGE_INTEGER Timeout100Ns;
    Timeout100Ns.QuadPart = Int32x32To64(TimeoutMs, -10000);
    return KeWaitForSingleObject(Event, Executive, KernelMode, FALSE, &Timeout100Ns);
}
#define WktEventWaitWithTimeout(Event, TimeoutMs) \
    (STATUS_SUCCESS == _WktEventWaitWithTimeout(&Event, TimeoutMs))

//
// Time Measurement Interfaces
//

//
// Returns the worst-case system timer resolution (in us).
//
inline
uint64_t
WktGetTimerResolution()
{
    ULONG MaximumTime, MinimumTime, CurrentTime;
    ExQueryTimerResolution(&MaximumTime, &MinimumTime, &CurrentTime);
    return NS100_TO_US(MaximumTime);
}

//
// Performance counter frequency.
//
extern uint64_t PlatformPerfFreq;

//
// Returns the current time in platform specific time units.
//
inline
uint64_t
WktTimePlat(
    void
    )
{
    return (uint64_t)KeQueryPerformanceCounter(NULL).QuadPart;
}

//
// Converts platform time to microseconds.
//
inline
uint64_t
WktTimePlatToUs64(
    uint64_t Count
    )
{
    //
    // Multiply by a big number (1000000, to convert seconds to microseconds)
    // and divide by a big number (PlatformPerfFreq, to convert counts to secs).
    //
    // Avoid overflow with separate multiplication/division of the high and low
    // bits. Taken from TcpConvertPerformanceCounterToMicroseconds.
    //
    uint64_t High = (Count >> 32) * 1000000;
    uint64_t Low = (Count & 0xFFFFFFFF) * 1000000;
    return
        ((High / PlatformPerfFreq) << 32) +
        ((Low + ((High % PlatformPerfFreq) << 32)) / PlatformPerfFreq);
}

//
// Converts microseconds to platform time.
//
inline
uint64_t
WktTimeUs64ToPlat(
    uint64_t TimeUs
    )
{
    uint64_t High = (TimeUs >> 32) * PlatformPerfFreq;
    uint64_t Low = (TimeUs & 0xFFFFFFFF) * PlatformPerfFreq;
    return
        ((High / 1000000) << 32) +
        ((Low + ((High % 1000000) << 32)) / 1000000);
}

#define WktTimeUs64() WktTimePlatToUs64(WktTimePlat())
#define WktTimeUs32() (uint32_t)WktTimeUs64()
#define WktTimeMs64() US_TO_MS(WktTimeUs64())
#define WktTimeMs32() (uint32_t)WktTimeMs64()

#define UNIX_EPOCH_AS_FILE_TIME 0x19db1ded53e8000ll

inline
int64_t
WktTimeEpochMs64(
    )
{
    LARGE_INTEGER SystemTime;
    KeQuerySystemTime(&SystemTime);
    return NS100_TO_MS(SystemTime.QuadPart - UNIX_EPOCH_AS_FILE_TIME);
}

//
// Returns the difference between two timestamps.
//
inline
uint64_t
WktTimeDiff64(
    _In_ uint64_t T1,     // First time measured
    _In_ uint64_t T2      // Second time measured
    )
{
    //
    // Assume no wrap around.
    //
    return T2 - T1;
}

//
// Returns the difference between two timestamps.
//
inline
uint32_t
WktTimeDiff32(
    _In_ uint32_t T1,     // First time measured
    _In_ uint32_t T2      // Second time measured
    )
{
    if (T2 > T1) {
        return T2 - T1;
    } else { // Wrap around case.
        return T2 + (0xFFFFFFFF - T1) + 1;
    }
}

//
// Returns TRUE if T1 came before T2.
//
inline
BOOLEAN
WktTimeAtOrBefore64(
    _In_ uint64_t T1,
    _In_ uint64_t T2
    )
{
    //
    // Assume no wrap around.
    //
    return T1 <= T2;
}

//
// Returns TRUE if T1 came before T2.
//
inline
BOOLEAN
WktTimeAtOrBefore32(
    _In_ uint32_t T1,
    _In_ uint32_t T2
    )
{
    return (int32_t)(T1 - T2) <= 0;
}

_IRQL_requires_max_(PASSIVE_LEVEL)
inline
void
WktSleep(
    _In_ uint32_t DurationMs
    )
{
    WKT_DBG_ASSERT(DurationMs != (uint32_t)-1);

    KTIMER SleepTimer;
    LARGE_INTEGER TimerValue;

    KeInitializeTimerEx(&SleepTimer, SynchronizationTimer);
    TimerValue.QuadPart = Int32x32To64(DurationMs, -10000);
    KeSetTimer(&SleepTimer, TimerValue, NULL);

    KeWaitForSingleObject(&SleepTimer, Executive, KernelMode, FALSE, NULL);
}

//
// Create Thread Interfaces
//

#define WKT_THREAD_FLAG_SET_IDEAL_PROC     0x0001
#define WKT_THREAD_FLAG_SET_AFFINITIZE     0x0002
#define WKT_THREAD_FLAG_HIGH_PRIORITY      0x0004

typedef struct WKT_THREAD_CONFIG {
    uint16_t Flags;
    uint8_t IdealProcessor;
    _Field_z_ const char* Name;
    KSTART_ROUTINE* Callback;
    void* Context;
} WKT_THREAD_CONFIG;

typedef struct _ETHREAD *WKT_THREAD;
#define WKT_THREAD_CALLBACK(FuncName, CtxVarName)  \
    _Function_class_(KSTART_ROUTINE)                \
    _IRQL_requires_same_                            \
    void                                            \
    FuncName(                                       \
      _In_ void* CtxVarName                         \
      )
#define WKT_THREAD_RETURN(Status) PsTerminateSystemThread(Status)
inline
NTSTATUS
WktThreadCreate(
    _In_ WKT_THREAD_CONFIG* Config,
    _Out_ WKT_THREAD* Thread
    )
{
    NTSTATUS Status;
    HANDLE ThreadHandle;
    Status =
        PsCreateSystemThread(
            &ThreadHandle,
            THREAD_ALL_ACCESS,
            NULL,
            NULL,
            NULL,
            Config->Callback,
            Config->Context);
    WKT_DBG_ASSERT(NT_SUCCESS(Status));
    if (!NT_SUCCESS(Status)) {
        *Thread = NULL;
        goto Error;
    }
    Status =
        ObReferenceObjectByHandle(
            ThreadHandle,
            THREAD_ALL_ACCESS,
            *PsThreadType,
            KernelMode,
            (void**)Thread,
            NULL);
    WKT_DBG_ASSERT(NT_SUCCESS(Status));
    if (!NT_SUCCESS(Status)) {
        *Thread = NULL;
        goto Cleanup;
    }
    if (Config->Flags & WKT_THREAD_FLAG_SET_IDEAL_PROC) {
        PROCESSOR_NUMBER Processor, IdealProcessor;
        WKT_TEL_ASSERT(Config->IdealProcessor < 64);
        Status =
            KeGetProcessorNumberFromIndex(
                Config->IdealProcessor,
                &Processor);
        WKT_DBG_ASSERT(NT_SUCCESS(Status));
        if (!NT_SUCCESS(Status)) {
            goto Cleanup;
        }
        IdealProcessor = Processor;
        Status =
            ZwSetInformationThread(
                ThreadHandle,
                ThreadIdealProcessorEx,
                &IdealProcessor, // Don't pass in Processor because this overwrites on output.
                sizeof(IdealProcessor));
        WKT_DBG_ASSERT(NT_SUCCESS(Status));
        if (!NT_SUCCESS(Status)) {
            goto Cleanup;
        }
        if (Config->Flags & WKT_THREAD_FLAG_SET_AFFINITIZE) {
            KAFFINITY Affinity = (KAFFINITY)(1ull << Config->IdealProcessor);
            Status =
                ZwSetInformationThread(
                    ThreadHandle,
                    ThreadAffinityMask,
                    &Affinity,
                    sizeof(Affinity));
            WKT_DBG_ASSERT(NT_SUCCESS(Status));
            if (!NT_SUCCESS(Status)) {
                goto Cleanup;
            }
        } else { // NUMA Node Affinity
            SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX Info;
            ULONG InfoLength = sizeof(Info);
            Status =
                KeQueryLogicalProcessorRelationship(
                    &Processor,
                    RelationNumaNode,
                    &Info,
                    &InfoLength);
            WKT_DBG_ASSERT(NT_SUCCESS(Status));
            if (!NT_SUCCESS(Status)) {
                goto Cleanup;
            }
            Status =
                ZwSetInformationThread(
                    ThreadHandle,
                    ThreadGroupInformation,
                    &Info.NumaNode.GroupMask,
                    sizeof(GROUP_AFFINITY));
            WKT_DBG_ASSERT(NT_SUCCESS(Status));
            if (!NT_SUCCESS(Status)) {
                goto Cleanup;
            }
        }
    }
    if (Config->Flags & WKT_THREAD_FLAG_HIGH_PRIORITY) {
        KeSetBasePriorityThread(
            (PKTHREAD)(*Thread),
            IO_NETWORK_INCREMENT + 1);
    }
    if (Config->Name) {
        DECLARE_UNICODE_STRING_SIZE(UnicodeName, 64);
        ULONG UnicodeNameLength = 0;
        Status =
            RtlUTF8ToUnicodeN(
                UnicodeName.Buffer,
                UnicodeName.MaximumLength,
                &UnicodeNameLength,
                Config->Name,
                (ULONG)strnlen(Config->Name, 64));
        WKT_DBG_ASSERT(NT_SUCCESS(Status));
        UnicodeName.Length = (USHORT)UnicodeNameLength;
#define ThreadNameInformation ((THREADINFOCLASS)38)
        Status =
            ZwSetInformationThread(
                ThreadHandle,
                ThreadNameInformation,
                &UnicodeName,
                sizeof(UNICODE_STRING));
        WKT_DBG_ASSERT(NT_SUCCESS(Status));
        if (!NT_SUCCESS(Status)) {
            goto Cleanup;
        }
    }
Cleanup:
    NtClose(ThreadHandle);
    if (!NT_SUCCESS(Status) && *Thread != NULL) {
        ObDereferenceObject(*Thread);
        *Thread = NULL;
    }
Error:
    return Status;
}
#define WktThreadDelete(Thread) ObDereferenceObject(*(Thread))
#define WktThreadWait(Thread) \
    KeWaitForSingleObject( \
        *(Thread), \
        Executive, \
        KernelMode, \
        FALSE, \
        NULL)
typedef ULONG_PTR WKT_THREAD_ID;
#define WktCurThreadID() ((WKT_THREAD_ID)PsGetCurrentThreadId())

//
// Processor Count and Index
//

#define WktProcMaxCount() KeQueryMaximumProcessorCountEx(ALL_PROCESSOR_GROUPS)
#define WktProcActiveCount() KeQueryActiveProcessorCountEx(ALL_PROCESSOR_GROUPS)
#define WktProcCurrentNumber() KeGetCurrentProcessorIndex()

//
// Rundown Protection Interfaces
//

typedef EX_RUNDOWN_REF WKT_RUNDOWN_REF;
#define WktRundownInitialize(Rundown) ExInitializeRundownProtection(Rundown)
#define WktRundownInitializeDisabled(Rundown) (Rundown)->Count = EX_RUNDOWN_ACTIVE
#define WktRundownReInitialize(Rundown) ExReInitializeRundownProtection(Rundown)
#define WktRundownUninitialize(Rundown)
#define WktRundownAcquire(Rundown) ExAcquireRundownProtection(Rundown)
#define WktRundownRelease(Rundown) ExReleaseRundownProtection(Rundown)
#define WktRundownReleaseAndWait(Rundown) ExWaitForRundownProtectionRelease(Rundown)

//
// Crypto Interfaces
//

//
// Returns cryptographically random bytes.
//
_IRQL_requires_max_(DISPATCH_LEVEL)
NTSTATUS
WktRandom(
    _In_ uint32_t BufferLen,
    _Out_writes_bytes_(BufferLen) void* Buffer
    );

//
// Silo interfaces
//

#define WKT_SILO PESILO
#define WKT_SILO_INVALID ((PESILO)(void*)(LONG_PTR)-1)

#define WktSiloGetCurrentServer() PsGetCurrentServerSilo()
#define WktSiloAddRef(Silo) if (Silo != NULL) { ObReferenceObjectWithTag(Silo, WKT_POOL_TAG); }
#define WktSiloRelease(Silo) if (Silo != NULL) { ObDereferenceObjectWithTag(Silo, WKT_POOL_TAG); }
#define WktSiloAttach(Silo) PsAttachSiloToCurrentThread(Silo)
#define WktSiloDetatch(PrevSilo) PsDetachSiloFromCurrentThread(PrevSilo)

//
// Network Compartment ID interfaces
//

#define WKT_COMPARTMENT_ID COMPARTMENT_ID

#define WKT_UNSPECIFIED_COMPARTMENT_ID UNSPECIFIED_COMPARTMENT_ID
#define WKT_DEFAULT_COMPARTMENT_ID     DEFAULT_COMPARTMENT_ID

COMPARTMENT_ID
NdisGetThreadObjectCompartmentId(
    IN PETHREAD ThreadObject
    );

NTSTATUS
NdisSetThreadObjectCompartmentId(
    IN PETHREAD ThreadObject,
    IN NET_IF_COMPARTMENT_ID CompartmentId
    );

#define WktCompartmentIdGetCurrent() NdisGetThreadObjectCompartmentId(PsGetCurrentThread())
#define WktCompartmentIdSetCurrent(CompartmentId) \
    NdisSetThreadObjectCompartmentId(PsGetCurrentThread(), CompartmentId)

#define WKT_CPUID(FunctionId, eax, ebx, ecx, dx)
