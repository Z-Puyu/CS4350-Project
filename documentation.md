# Gameplay Abilities System: Documentation

# Modifiers

This section covers **modifiers**, the main component to dynamically change run-time attributes of game objects.
## Classes

### Modifier

**Namespace:** `GameplayAbilities.Runtime.Modifiers`

Represents a modifier that can be applied to a game attribute, supporting different operation types and arithmetic operations.

#### Operation Types

A modifier can be of either of the following types:

- `Shift`: Adds or subtracts from the base value (flat modification)
- `Multiply`: Multiplies the base value by a percentage (e.g., a multiplier of 10 will modify the value to 110% of the base value)
    
    Multipliers are **added before applied** to the base value, i.e., two +10% multipliers are composed to +20% instead of +21%.
- `Offset`: Adds or subtracts from the final value (applied after all other modifications), reasonable to be used for things like health reduction

#### Properties

- `Type`: The operation type (Shift, Multiply, or Offset)
- `Target`: The attribute ID this modifier affects
- `Magnitude`: The strength of the modification

#### Methods

- `float Modify(float value)`: Applies the modifier to the given value and returns the modified value

#### Operators

The modifier class comes with support for basic arithmetic operators.

- `-(Modifier modifier)`: Negates the magnitude of a modifier
- `+(Modifier a, Modifier b)`: Adds two modifiers of the same type and target
- `-(Modifier a, Modifier b)`: Subtracts one modifier from another of the same type and target
- `*(Modifier modifier, float multiplier)`: Multiplies a modifier's magnitude by a scalar value
- `*(float multiplier, Modifier modifier)`: Same as above
- `/(Modifier modifier, float divisor)`: Divides a modifier's magnitude by a scalar value

---

### ModifierData

**Namespace:** `GameplayAbilities.Runtime.Modifiers`

A serializable class that defines modifier configuration data, used to create `Modifier` instances at runtime. This is used to define how modifiers should be created and applied for **gameplay effects**.

The magnitude of a modifier can be of different forms:

- **Constant**: a fixed static value
- **Attribute Value**: uses the value of an attribute from a game object
- **Caller Supplied Value**: a value provided at runtime when a gameplay effect is executed

#### Properties

- `TargetAttribute`: The attribute type this modifier affects
- `Method`: The operation type (Shift, Multiply, or Offset)
- `UseAttributeValue`: Whether to use an attribute value instead of a constant
- `Source`: Which attribute set to use when `UseAttributeValue` is true (either the **target** or **instigator** of a gameplay effect)
- `SourceAttribute`: The attribute whose value should be used when `UseAttributeValue` is true
- `AllowSetByCaller`: Whether this value should be set by the caller at runtime
- `Value`: The default constant value to use
- `Label`: Identifier for caller-supplied modifier values

#### Methods

- `Modifier CreateModifier(AttributeSet target, GameplayEffectExecutionArgs args)`: Creates a Modifier instance based on the configuration and execution context