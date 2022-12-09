
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Saltyfish.Util;
using Saltyfish.Data;
using Saltyfish.UI;
using Cysharp.Text;

namespace Saltyfish.Archive
{
    public class ArchiveManager : Singleton<ArchiveManager>
    {
        public const int MAX_SAVEGAME_COUNT = 3;

        public static readonly string ArchivePath = Path.Combine(Application.persistentDataPath, "SaveGame/");

        private static readonly string[] SaveGameNames = new string[MAX_SAVEGAME_COUNT]
        {
            "Save1",
            "Save2",
            "Save3",
        };

        public event Action<int, GameData> OnArchiveCreate;

        public event Action<int, GameData> OnArchiveDelete;

        private GameData[] m_SaveGames = new GameData[MAX_SAVEGAME_COUNT];

        public GameData[] ArrSaveGames
        {
            get
            {
                return m_SaveGames;
            }
        }

        public bool IsFull
        {
            get
            {
                for (int i = 0; i < m_SaveGames.Length; ++i)
                {
                    if (m_SaveGames[i] == null)
                        return false;
                }
                return true;
            }
        }

        public bool HasSaveGame
        {
            get
            {
                for (int i = 0; i < m_SaveGames.Length; ++i)
                {
                    if (m_SaveGames[i] != null)
                        return true;
                }
                return false;
            }
        }

        public override void Init()
        {
            EnsureDirExist();
            InitializeSaveFiles();
        }

        void EnsureDirExist()
        {
            if (!Directory.Exists(ArchivePath))
            {
                Directory.CreateDirectory(ArchivePath);
            }
        }

        void InitializeSaveFiles()
        {
            var arrFile = Directory.GetFiles(ArchivePath, "*.KBSaveGame");
            Array.Sort(arrFile);
            var fileNum = Math.Min(MAX_SAVEGAME_COUNT, arrFile.Length);
            try
            {
                for (int i = 0; i < fileNum; ++i)
                {
                    var text = File.ReadAllText(arrFile[i]);
                    var obj = JsonConvert.DeserializeObject<GameData>(text);
                    if (obj != null)
                    {
                        m_SaveGames[i] = obj;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.ShowNotice("存档数据异常，请尝试新建存档或者联系开发者");
                Debug.LogError(ex);
            }
        }

        public GameData DefaultGame
        {
            get
            {
                var saveGame = GetSaveGame(0);
                if (saveGame == null)
                    TryCreateNew(0, out saveGame);
                return saveGame;
            }
        }

        public GameData GetSaveGame(string fileName)
        {
            return Array.Find(m_SaveGames, game => game != null && game.FileName == fileName);
        }

        public bool Contains(GameData saveData)
        {
            if (saveData == null)
                return false;
            return Array.FindIndex(m_SaveGames, game => saveData == game) != -1;
        }

        public GameData GetSaveGame(int idx)
        {
            if (idx < 0 || idx > m_SaveGames.Length)
                return null;
            return m_SaveGames[idx];
        }

        public bool ExistGame(string fileName)
        {
            return GetSaveGame(fileName) != null;
        }

        public bool DeleteSaveGame(int idx)
        {
            if (idx < 0 || idx >= m_SaveGames.Length)
                return false;
            if (m_SaveGames[idx] == null)
                return false;
            var saveData = m_SaveGames[idx];
            m_SaveGames[idx] = null;
            saveData.Delete();
            OnArchiveDelete?.Invoke(idx, saveData);
            return true;
        }

        public bool DeleteSaveGame(GameData saveData)
        {
            if (saveData == null)
                return false;
            for (int i = 0; i < m_SaveGames.Length; ++i)
            {
                if (m_SaveGames[i] == saveData)
                {
                    m_SaveGames[i] = null;
                    saveData.Delete();
                    OnArchiveDelete?.Invoke(i, saveData);
                    return true;
                }
            }
            return false;
        }

        public bool TryCreateNew(int idx, out GameData data)
        {
            data = null;
            if (IsFull)
                return false;
            if (idx < 0 || idx >= m_SaveGames.Length)
                return false;
            if (m_SaveGames[idx] != null)
            {
                data = m_SaveGames[idx];
                return true;
            }
            data = new GameData()
            {
                FileName = AllocateName(idx),
                Version = Application.version,
                CreateDateTimeTick = DateTime.Now.Ticks,
            };
            data.ResetGameData();
            data.Save();
            m_SaveGames[idx] = data;
            OnArchiveCreate?.Invoke(idx, data);
            return true;
        }

        private string AllocateName(int idx)
        {
            return ZString.Format("Save{0}", idx + 1);
        }

        public override void DeInit()
        {
            Array.Clear(m_SaveGames, 0, m_SaveGames.Length);
        }
    }
}
