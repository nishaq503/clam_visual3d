use crate::utils::distances::DistanceMetric;
use crate::utils::error::FFIError;
use crate::utils::helpers;

#[repr(C)]
pub struct StartupDataFFI {
    pub data_name: *const u8,
    pub name_len: i32,
    pub distance_metric: DistanceMetric,
    pub cardinality: u32,
    pub is_expensive: bool,
}

pub struct StartupData {
    pub data_name: String,
    pub distance_metric: DistanceMetric,
    pub cardinality: u32,
    pub is_expensive: bool,
}

impl StartupData {
    pub unsafe fn from_ffi_checked(data: &StartupDataFFI) -> Result<StartupData, FFIError> {
        match helpers::csharp_to_rust_utf8(data.data_name, data.name_len) {
            Ok(data_name) => Ok(StartupData {
                data_name,
                distance_metric: data.distance_metric,
                cardinality: data.cardinality,
                is_expensive: data.is_expensive,
            }),
            Err(e) => Err(e),
        }
    }
}
