use crate::ffi_impl::cluster_ids::ClusterIDs;
use crate::utils::types::Clusterf32;

pub struct ClusterIDsWrapper {
    data: ClusterIDs,
}

impl Drop for ClusterIDsWrapper {
    fn drop(&mut self) {
        // debug!("Freeing Cluster Data string with rust destructor");
        self.data.free_ids();
    }
}

impl ClusterIDsWrapper {
    pub fn from_cluster(cluster: &Clusterf32) -> Self {
        ClusterIDsWrapper {
            data: ClusterIDs::from_clam(cluster),
        }
    }

    pub fn data(&self) -> &ClusterIDs {
        &self.data
    }
    pub fn data_mut(&mut self) -> &mut ClusterIDs {
        &mut self.data
    }
}
