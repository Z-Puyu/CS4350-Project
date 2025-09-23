using System.Collections.Generic;
using DataStructuresForUnity.Runtime.GeneralUtils;
using UnityEngine;

namespace Game.Map {
    public sealed class GameWorldManager : Singleton<GameWorldManager> {
        [field: SerializeField] private GameWorld MainWorld { get; set; }
        [field: SerializeField] private GameWorld PurgatoryWorld { get; set; }
        
        public static GameWorld Main => Singleton<GameWorldManager>.Instance.MainWorld;
        public static GameWorld Purgatory => Singleton<GameWorldManager>.Instance.PurgatoryWorld;
        
        public static bool IsInPurgatory => 
                Singleton<GameWorldManager>.Instance.PurgatoryWorld.gameObject.activeInHierarchy;
    }
}
