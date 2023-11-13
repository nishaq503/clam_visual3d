use crate::ffi_impl::string_ffi::StringFFI;
use crate::utils::distances::DistanceMetric;

#[repr(C)]
#[derive(Copy, Clone, Debug)]
pub struct TreeStartupDataFFI {
    pub data_name: StringFFI,
    pub distance_metric: DistanceMetric,
    pub cardinality: u32,
    pub is_expensive: bool,
    pub should_load: bool,
}
