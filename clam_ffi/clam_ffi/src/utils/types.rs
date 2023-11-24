use abd_clam::{Cakes, Cluster, VecDataset};

use crate::handle::handle::Handle;

pub type OutHandlePtr<'a> = Option<&'a mut *mut Handle<'a>>;

pub type InHandlePtr<'a> = Option<&'a mut Handle<'a>>;

pub type Clusterf32 = Cluster<f32>;
pub type DataSet = VecDataset<Vec<f32>, f32, bool>;
pub type Cakesf32 = Cakes<Vec<f32>, f32, DataSet>;
