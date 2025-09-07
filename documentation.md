# Gameplay Abilities System

This system serves as a framework for implementing game entity interactions based on abilities/skills. It supports a variety of effects and an extensive stats system.

This system is heavily inspired by Unreal Engine's Gameplay Ability System.

## Core Functionality

- Manages abilities (`IAbility`) and perks (`Perk`) for a game entity
- Handles the execution of abilities and their effects on target entities
- Manages the lifecycle of gameplay effects

## Main Concepts

- **Ability**: Represents a game action that can be executed by the player, e.g. a skill
- **Perk**: Represents a wrapper around attainable abilities and buffs
- **Attribute Set**: A manager for a game entity's stats and attributes to reflect the effects of an ability
- **Gameplay Effect**: A wrapper around an effect applied to the target entity when using an ability
- **Modifier**: A run-time change to an attribute value, usually due to a gameplay effect

## Usage Example

Overall, the system has the following internal structure:

- An **ability system** manages **abilities**, **perks** and has an **attribute set**.
- An **attribute set** manages all **attributes** (i.e., gameplay stats) of a game entity.
- A **modifier** modifies the value of an **attribute** at run-time.
- A **gameplay effect** applies a series of **modifiers** to an **attribute set**.
- A **perk** applies **modifiers** and grants **abilities** on activation.
- An **ability** applies a series of **gameplay effects** to a target entity.

```csharp
// Get references to the ability systems
AbilitySystem playerAbilitySystem = player.GetComponent<AbilitySystem>();
AbilitySystem enemyAbilitySystem = enemy.GetComponent<AbilitySystem>();
IAbility fireballAbility = // ... get fireball ability

// Use the fireball ability on the enemy
playerAbilitySystem.Use(fireballAbility, enemyAbilitySystem);

// Enable a perk
Perk fireMastery = // ... get fire mastery perk
playerAbilitySystem.Enable(fireMastery);
```

## Classes

### AbilitySystem

**Namespace:** `GameplayAbilities.Runtime.Abilities`

#### Methods

- `void Enable(Perk perk)`: Activates a perk, applying its modifiers and granting its abilities
- `void Disable(Perk perk)`: Deactivates a perk, removing its modifiers and revoking its abilities
- `void Grant(IAbility ability)`: Adds an ability to the system
- `void Revoke(IAbility ability)`: Removes an ability from the system
- `void Use(IAbility ability, AbilitySystem target)`: Attempts to use an ability on a target
- `void AddEffect(GameplayEffectData effect)`: Adds a gameplay effect to the system

#### Events

The following events are used to signal the start and end of an ability's execution. These can be easily used to coordinate cosmetic effects such as particles, sound effects or shaders.

- `UnityEvent<IAbility> OnStartAbility`: Triggered when an ability starts being used
- `UnityEvent<IAbility> OnEndAbility`: Triggered when an ability finishes its execution

---

### Ability

**Namespace:** `GameplayAbilities.Runtime.Abilities`

A wrapper for an ability that can be used by a game entity. Allows to specify multiple effects to be applied when executed. You can also implement the `IAbility` interface on your own classes to create different ability types, though that may not be necessary.

#### Methods

- `IEnumerable<GameplayEffect> GenerateEffects(GameplayEffectExecutionArgs args)`: Generates a list of gameplay effects based on the provided execution arguments.
- `bool IsUsable(AttributeSet instigator, AttributeSet target)`: Checks if the ability can be used by the instigator.

---

### Perk

**Namespace:** `GameplayAbilities.Runtime.Abilities`

A wrapper for a perk that can be enabled or disabled by a game entity. Allows you to specify a list of buffs/debuffs to be applied and a list of abilities to be granted when the perk is activated.

This class is a data-only class. It does not perform any logic on its own and must be used in conjunction with the `AbilitySystem` class, which will manage the activation and deactivation of perks.

---

### AttributeTypeDefinition

**Namespace:** `GameplayAbilities.Runtime.Attributes`

Defines the type and behavior of an attribute. This ScriptableObject allows you to create different types of attributes with custom modification rules and hierarchical organization.

#### Properties

- `string Name`: The name of the attribute type
- `string DisplayName`: The display name of the attribute (falls back to `Name` if not set), useful for UI
- `string Id`: The unique identifier for this attribute (auto-generated based on hierarchy). For example, a **fire magic resistance** attribute may have ID **"Resistance.Magic.Fire"**
- `List<IAttributeClampRule> ModificationRules`: Rules that define how this attribute's values should be clamped
- `List<AttributeTypeDefinition> SubTypes`: Child attribute types, allowing for hierarchical organisation

#### Methods

- `bool Includes(string attribute)`: Checks if `attribute` is a subtype under this attribute type

---

### Attribute

**Namespace:** `GameplayAbilities.Runtime.Attributes`

A simple data structure that represents an attribute with a name and an integer value. This is used as a lightweight way to pass attribute information around the system.

#### Properties

- `string Name`: The name/identifier of the attribute. Note that this is the full path-qualified name.
- `int Value`: The current value of the attribute

---

### AttributeChange

**Namespace:** `GameplayAbilities.Runtime.Attributes`

Represents a change in an attribute's value. This is used in the `OnAttributeChanged` event of `AttributeSet` to provide information about attribute modifications.

#### Properties

- `string AttributeName`: The full path-qualified name of the attribute that changed
- `int OldValue`: The value of the attribute before the change
- `int CurrentValue`: The new value of the attribute after the change

---

### AttributeTable

**Namespace:** `GameplayAbilities.Runtime.Attributes`

A ScriptableObject that defines the initial values for a set of attributes. This is used to initialize an `AttributeSet` with starting values for different game entities.

---

### IAttributeReader

**Namespace:** `GameplayAbilities.Runtime.Attributes`

This interface is used to read attribute values from an `AttributeSet`. It is mainly used for child game objects which require attribute data from a parent object during run-time.

#### Methods

- `int GetCurrent(string key)`: Gets the current value of an attribute
- `int GetMax(string key)`: Gets the maximum value of an attribute
- `int GetMin(string key)`: Gets the minimum value of an attribute
- `Attribute GetAttribute(string key)`: Gets an `Attribute` object containing the current value of the specified attribute
- `bool Has(int threshold, string key)`: Checks if the current value of an attribute meets or exceeds a threshold

---

### AttributeReader

**Namespace:** `GameplayAbilities.Runtime.Attributes`

A simple implementation of `IAttributeReader` that provides access to an `AttributeSet`'s attributes.

---

### AttributeSet

**Namespace:** `GameplayAbilities.Runtime.Attributes`

A component that manages a collection of attributes (stats) for a game entity. It handles the storage, modification, and event notification for attribute changes. This is a core component that works in conjunction with the `AbilitySystem` to manage an entity's stats and their modifications.

This class implements `IAttributeReader`.

#### Key Features

- Manages a collection of attributes with their current, minimum, and maximum values
- Handles attribute modification through modifiers
- Invokes notification events when attributes are modified
- Supports attribute initialisation using an `AttributeTable`
- Implements efficient attribute lookup by path-qualified ID using a trie data structure

#### Properties

- `OnAttributeChanged`: Event triggered when an attribute's value changes

#### Methods

- `void Initialise(AttributeTable table)`: Initializes the attribute set with values from an `AttributeTable`
- `int GetCurrent(string key)`: Gets the current value of an attribute
- `int GetMax(string key)`: Gets the maximum value of an attribute
- `int GetMin(string key)`: Gets the minimum value of an attribute
- `Attribute GetAttribute(string key)`: Gets an `Attribute` object containing the current value of the specified attribute
- `bool Has(int threshold, string key)`: Checks if the current value of an attribute meets or exceeds a threshold

#### Usage Example

```csharp
// Get reference to the attribute set
AttributeSet attributeSet = GetComponent<AttributeSet>();

// Get current health value
int currentHealth = attributeSet.GetCurrent("Health");

// Get a attribute that sits deeper in hierarchy
Attribute fireMagicResistance = attributeSet.GetAttribute("Resistance.Magic.Fire");

// Check if health is above 50
bool isHealthy = attributeSet.Has(50, "Health");

// Subscribe to attribute changes
attributeSet.OnAttributeChanged += change => {
    Debug.Log($"{change.AttributeName} changed from {change.OldValue} to {change.CurrentValue}");
};
```

#### Important Notes

- The `AttributeSet` should be initialised with an `AttributeTable` before use
- The class implements `IEnumerable<Attribute>` for easy iteration over all attributes
- Attributes are stored in a trie for efficient prefix-based lookups

---

## Modifiers

**Namespace:** `GameplayAbilities.Runtime.Modifiers`

This section covers **modifiers**, the main component to dynamically change run-time attributes of game objects.

A modifier can be of either of the following types:

- `Shift`: Adds or subtracts from the base value (flat modification)
- `Multiply`: Multiplies the base value by a percentage (e.g., a multiplier of 10 will modify the value to 110% of the base value)

  Multipliers are **added before applied** to the base value, i.e., two +10% multipliers are composed to +20% instead of +21%.
- `Offset`: Adds or subtracts from the final value (applied after all other modifications), reasonable to be used for things like health reduction

The formula to compute the final attribute value based on modifiers is:

$$
\textrm{final} = (\textrm{base} + \textrm{shift}) * \textrm{multiplier} + \textrm{offset}
$$

#### Properties

- `Type`: The operation type (Shift, Multiply, or Offset)
- `Target`: The attribute ID this modifier affects
- `Magnitude`: The strength of the modification

#### Methods

- `float Modify(float value)`: Applies the modifier to the given value and returns the modified value

#### Operators

The modifier class comes with support for basic arithmetic operators.

- `Modifier -(Modifier modifier)`: Negates the magnitude of a modifier
- `Modifier +(Modifier a, Modifier b)`: Adds two modifiers of the same type and target
- `Modifier -(Modifier a, Modifier b)`: Subtracts one modifier from another of the same type and target
- `Modifier *(Modifier modifier, float multiplier)`: Multiplies a modifier's magnitude by a scalar value
- `Modifier *(float multiplier, Modifier modifier)`: Same as above
- `Modifier /(Modifier modifier, float divisor)`: Divides a modifier's magnitude by a scalar value

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

---

### GameplayEffect

**Namespace:** `GameplayAbilities.Runtime.Effects`

A wrapper for a gameplay effect that can be applied to a game entity. Allows you to specify a list of modifiers to be applied when the effect is executed.

When an effect is applied, there could be three possible outcomes:

- `Success`: The effect was successfully applied to the target.
- `Failure`: The effect was not applied to the target. Use this to implement mechanics like **missed attack**, **dodge**, **elemental resistance**, etc.
- `Cancelled`: The effect was cancelled before being applied to the target. Use this to implement mechanics like **mana cost**.

---

### GameplayEffectData

**Namespace:** `GameplayAbilities.Runtime.Effects`

A **serialisable** class that represents a gameplay effect's configuration. It is used to store the data of a gameplay effect, such as the modifiers to be applied and the execution time.

It also defines the type of the effect, which can be one of the following:

- `Instant`: The effect is applied **immediately and once** when the ability is used.
- `Periodic`: The effect is applied **periodically** at a fixed interval. Use this for status effects.
- `Continuous`: The effect is applied **for a fixed duration**. Use this for buffs/debuffs.

#### Properties

- `ExecutionTime`: Determines the type of the effect.
- `Periodicity`: The interval at which the effect is applied when `ExecutionTime` is `Periodic`.
- `Duration`: The duration for which the effect is applied when `ExecutionTime` is `Continuous`.
- `BaseChance`: The base chance of the effect to be applied. This is used to implement mechanics like **missed attack**, **dodge**, **elemental resistance**, etc.
- `Costs`: The costs to apply the effect.
- `Executor`: Determines how this effect should be executed. The default executor applies a series of pre-defined modifiers.
- `HasLevel`: Whether this effect's strength is affected by ability level.
- `LevelEffect`: A curve to map a level to a coefficient when `HasLevel` is `true`, which will be used to scale the magnitude of the effect.

---

### EffectCommitmentCost

**Namespace:** `GameplayAbilities.Runtime.GameplayEffects`

Defines a resource cost required to apply a gameplay effect, with configurable affordability policies.

#### Properties

- `AttributeTypeDefinition Attribute`: The attribute to check/consume
- `int Value`: The amount of the attribute to consume
- `bool WillAddInsteadOfUse`: Whether to add to the attribute instead of consuming it
- `AffordabilityPolicy Affordability`: The rule for determining if the cost can be paid

#### AffordabilityPolicy Enum

- `HaveStrictlyMore`: Attribute must be strictly greater than the cost
- `HaveEnough`: Attribute must be greater than or equal to the cost
- `HaveAny`: Attribute must be non-zero
- `WillNotHitLimit`: For additive effects, ensures adding won't hit the attribute's maximum
- `WillNotOverflow`: For additive effects, ensures adding won't exceed the attribute's maximum
- `HaveRoomForMore`: For additive effects, only checks if the attribute isn't at maximum

#### Methods

- `bool IsAffordable(IAttributeReader source)`: Checks if the cost can be paid

---

### GameplayEffectExecutionArgs

**Namespace:** `GameplayAbilities.Runtime.GameplayEffects`

Contains contextual information for executing a gameplay effect, such as the instigator and any runtime parameters.

#### Properties

- `IAttributeReader Instigator`: The entity that initiated the effect
- `float Level`: The level of the effect
- `Dictionary<string, int> CallerSuppliedModifierValues`: Runtime values for effect modifiers

#### Builder Pattern

Use the nested `Builder` class to create instances:

```csharp
var args = GameplayEffectExecutionArgs.Builder
    .From(instigator)
    .WithLevel(5)
    .WithModifier(10, "damage") // set this for modifiers that require their magnitude to be set at run-time
    .Build();
```

### EffectExecution

**Namespace:** `GameplayAbilities.Runtime.GameplayEffects.Executions`

An abstract class that defines the execution logic for a gameplay effect. You should override this class if you need your custom execution logic for gameplay effects.

#### Methods

- `void Execute(GameplayEffect effect, GameplayEffectExecutionArgs args)`: Executes the gameplay effect
- `virtual void OnFirstExecution(AttributeSet target, GameplayEffectExecutionArgs args)`: Additional logic to execute on the first execution of the gameplay effect
- `virtual bool Try(IAttributeReader target, int chance, GameplayEffectExecutionArgs args)`: Tries to execute the gameplay effect. By default this rolls a random number for probability test. You can add extra logic here to determine when an effect is considered as "missed" or "dodged".

