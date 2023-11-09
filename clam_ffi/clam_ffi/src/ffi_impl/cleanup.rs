use super::cluster_data::ClusterData;
use crate::ffi_impl::cluster_ids::ClusterIDs;

pub trait Cleanup {
    fn free_ids(&mut self) {
        self.free_ids();
    }
}

impl Cleanup for ClusterIDs {
    fn free_ids(&mut self) {
        self.id.free();
        self.left_id.free();
        self.right_id.free();
    }
}

impl Cleanup for ClusterData {
    fn free_ids(&mut self) {
        self.id.free();
    }
}
