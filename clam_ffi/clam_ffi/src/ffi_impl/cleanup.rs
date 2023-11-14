use crate::debug;
use super::cluster_data::ClusterData;
use crate::ffi_impl::cluster_ids::ClusterIDs;
use crate::ffi_impl::string_ffi::StringFFI;

pub trait Cleanup {
    fn free(&mut self);
}

impl Cleanup for ClusterIDs {
    fn free(&mut self) {
        self.id.free();
        self.left_id.free();
        self.right_id.free();
    }
}

impl Cleanup for ClusterData {
    fn free(&mut self) {
        self.id.free();
    }
}

impl Cleanup for StringFFI{
    fn free(&mut self) {
        // debug!("freed from cleanup sffi");
        self.free();
    }
}
