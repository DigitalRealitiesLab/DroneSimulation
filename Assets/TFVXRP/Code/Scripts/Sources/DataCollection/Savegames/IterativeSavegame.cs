using System;
using UnityEngine;

namespace TFVXRP.DataCollection.Savegames {
    [Serializable]
    internal abstract class IterativeSavegame : Savegame {
        [SerializeField]
        protected string _id;

        protected static TSavegame Initialize<TSavegame>(string id) where TSavegame : IterativeSavegame, new() {
            var savegame = new TSavegame {
                _id = id
            };
            return savegame;
        }
    }
}