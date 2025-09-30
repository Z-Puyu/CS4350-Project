namespace Weapons.Runtime {
    public abstract class WeaponStats {
        public enum ModifierType { Shift, Multiply, Offset }
        
        public abstract void Set(string key, int value);
        
        public abstract int Get(string key);
        
        public abstract void Modify(string key, int modifier, ModifierType type);
    }
}
