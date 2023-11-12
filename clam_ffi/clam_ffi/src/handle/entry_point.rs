use crate::handle::handle::Handle;
use crate::utils::distances::DistanceMetric;
use crate::utils::helpers;
use crate::utils::types::{InHandlePtr, OutHandlePtr};

use crate::utils::error::FFIError;

use crate::debug;
use crate::ffi_impl::startup_data::{StartupData, StartupDataFFI};

pub unsafe fn shutdown_clam_impl(context_ptr: OutHandlePtr) -> FFIError {
    if let Some(handle) = context_ptr {
        let _ = Box::from_raw(*handle);
        return FFIError::Ok;
    } else {
        debug!("shtudown clam handle not valid?");
        return FFIError::NullPointerPassed;
    }
}

pub unsafe fn force_physics_shutdown(ptr: InHandlePtr) -> i32 {
    // Handle::from_ptr(ptr).get_num_nodes() + 1

    if let Some(handle) = ptr {
        // debug!("cardinality: {}", handle.tree_height() + 1);
        handle.force_physics_shutdown();

        // return handle.tree_height() + 1;
    }
    debug!("handle not created");

    return 0;
}

pub unsafe fn init_clam_impl(
    ptr: OutHandlePtr,
    // data_name: *const u8,
    // name_len: i32,
    // cardinality: u32,
    // distance_metric: DistanceMetric,
    data : Option<&StartupDataFFI>
) -> FFIError {
    // let data_name = match helpers::csharp_to_rust_utf8(data_name, name_len) {
    //     Ok(data_name) => data_name,
    //     Err(e) => {
    //         debug!("{:?}", e);
    //         return FFIError::InvalidStringPassed;
    //     }
    // };

    let startup_data = match StartupData::from_ffi_checked(data.unwrap()) {
        Ok(data) => data,
        Err(e) => {
            debug!("{:?}", e);
            return e;
        }
    };

    match Handle::new(&startup_data) {
        Ok(handle) => {
            if let Some(out_handle) = ptr {
                *out_handle = Box::into_raw(Box::new(handle));
            }

            debug!("built clam tree for {}", startup_data.data_name);
            return FFIError::Ok;
        }
        Err(e) => {
            debug!("{:?}", e);
            return FFIError::HandleInitFailed;
        }
    }
}

pub unsafe fn load_cakes_impl(ptr: OutHandlePtr, startup_data_ffi: &StartupDataFFI) -> FFIError {
   let startup_data = match  StartupData::from_ffi_checked(startup_data_ffi){
        Ok(data) => data,
        Err(e) => {
            debug!("{:?}", e);
            return e;
        }
   };

    debug!("loaded data name succesfuly");


    match Handle::load(&startup_data) {
        Ok(handle) => {
            debug!("loaded handle succesfuly");

            if let Some(out_handle) = ptr {
                *out_handle = Box::into_raw(Box::new(handle));
            }

            debug!("built clam tree for {}", startup_data.data_name);
            return FFIError::Ok;
        }
        Err(e) => {
            debug!("handle build failed {:?}", e);
            return FFIError::HandleInitFailed;
        }
    }
}
