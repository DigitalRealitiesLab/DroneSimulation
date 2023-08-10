using Support.Extensions;

namespace TFVXRP.Scenery {
    /// <summary>
    ///     The different Scene modes
    /// </summary>
    internal enum SceneType {
        /// <summary>
        ///     Both the scenery/world/environment and the entities within it are virtual.
        /// </summary>
        [EnumExtensions.StringRepresentation("VR")]
        VR,

        /// <summary>
        ///     The scenery/world/environment is real while the entities within it are virtual.
        /// </summary>
        [EnumExtensions.StringRepresentation("AR")]
        AR,

        /// <summary>
        ///     Both the scenery/world/environment and the entities within it are real.
        /// </summary>
        [EnumExtensions.StringRepresentation("Reality")]
        Reality,

        /// <summary>
        ///     Same as <see cref="Reality" />, but without a HMD.
        /// </summary>
        [EnumExtensions.StringRepresentation("RealityWithoutHMD")]
        RealityWithoutHmd
    }
}