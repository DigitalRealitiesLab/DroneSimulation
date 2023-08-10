using System;

namespace TFVXRP.Scenery {
    internal interface IRoom {
        internal Func<(bool, bool)> GetSceneryRealism { get; set; }
    }
}