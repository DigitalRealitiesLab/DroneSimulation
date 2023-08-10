using System;
using System.IO;
using System.Linq;
using Support.Constants;
using Support.Extensions;
using UnityEngine;

namespace TFVXRP.DataCollection.Savegames {
    [Serializable]
    internal abstract class Savegame {
        protected static string defaultFolderPath = ToPath(Application.persistentDataPath, "Savegames");

        protected static string ToPath(params string[] pathParts) => pathParts.Aggregate(string.Empty, (current, entry) => current + "/" + entry).Trim('/');

        private static (string FolderPath, string FilePath) ToThisPath(string folderName, string filename) {
            string folderPath = folderName.IsNotEmpty() ? ToPath(defaultFolderPath, folderName) : defaultFolderPath;
            string filePath = ToPath(folderPath, filename + ".json");
            return (folderPath, filePath);
        }

        protected void Save(string filename, string folderName = Constants.EMPTY_STRING) {
            (string FolderPath, string FilePath) paths = ToThisPath(folderName, filename);

            if (!Directory.Exists(paths.FolderPath)) {
                Directory.CreateDirectory(paths.FolderPath);
            }

            File.WriteAllText(paths.FilePath, JsonUtility.ToJson(this));
        }

        protected static bool Load<TSavegame>(out TSavegame savegame, string filename, string folderName = Constants.EMPTY_STRING) where TSavegame : Savegame {
            (string FolderPath, string FilePath) paths = ToThisPath(folderName, filename);

            if (File.Exists(paths.FilePath)) {
                savegame = JsonUtility.FromJson<TSavegame>(File.ReadAllText(paths.FilePath));
                return true;
            }

            savegame = null;
            return false;
        }
    }
}