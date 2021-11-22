/*++

    Copyright (c) Microsoft Corporation.
    Licensed under the MIT License.

Abstract:

    Main entry point to the driver.

--*/

#include "wkt.h"

INITCODE
_IRQL_requires_max_(PASSIVE_LEVEL)
void
CoreLibraryLoad(
    void
    );

_IRQL_requires_max_(PASSIVE_LEVEL)
void
CoreLibraryUnload(
    void
    );

INITCODE DRIVER_INITIALIZE DriverEntry;
PAGEDX EVT_WDF_DRIVER_UNLOAD EvtDriverUnload;

INITCODE
_Function_class_(DRIVER_INITIALIZE)
_IRQL_requires_same_
_IRQL_requires_(PASSIVE_LEVEL)
NTSTATUS
DriverEntry(
    _In_ PDRIVER_OBJECT DriverObject,
    _In_ PUNICODE_STRING RegistryPath
    )
{
    NTSTATUS Status;
    WDF_DRIVER_CONFIG Config;
    WDFDRIVER Driver;

    PlatformSystemLoad(DriverObject, RegistryPath);
    CoreLibraryLoad();

    //
    // Create the WdfDriver Object.
    //
    WDF_DRIVER_CONFIG_INIT(&Config, NULL);
    Config.EvtDriverUnload = EvtDriverUnload;
    Config.DriverInitFlags = WdfDriverInitNonPnpDriver;
    Config.DriverPoolTag = WKT_POOL_TAG;

    Status =
        WdfDriverCreate(
            DriverObject,
            RegistryPath,
            WDF_NO_OBJECT_ATTRIBUTES,
            &Config,
            &Driver);
    if (!NT_SUCCESS(Status)) {
        goto Error;
    }

Error:

    if (!NT_SUCCESS(Status)) {
        CoreLibraryUnload();
        PlatformSystemUnload();
    }

    return Status;
}

PAGEDX
_Function_class_(EVT_WDF_DRIVER_UNLOAD)
_IRQL_requires_same_
_IRQL_requires_max_(PASSIVE_LEVEL)
void
EvtDriverUnload(
    _In_ WDFDRIVER Driver
    )
{
    UNREFERENCED_PARAMETER(Driver);
    PAGED_CODE();
    CoreLibraryUnload();
    PlatformSystemUnload();
}
