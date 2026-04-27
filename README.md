# Worlds Without Number Character Sheet

A digital character sheet application for the Worlds Without Number tabletop RPG system, built with React frontend and .NET backend.

## Overview

This application provides a comprehensive web-based character sheet for managing Worlds Without Number characters, including character creation, attribute tracking, skill management, inventory, spells, and combat calculations.

## Feature Status

### Implemented Features ✓

#### Core Character Management
- Character identity (name, background, origin, class)
- Multi-class support (Warrior, Expert, Mage, Adventurer with partial class combinations)
- Level tracking (1-10)
- Experience points management

#### Attributes & Skills
- Six core attributes (Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma)
- Attribute modifier calculations
- 16 base skills plus custom skill support
- Individual skill rank tracking

#### Combat System
- Armor Class calculation
- Base Attack Bonus (BAB) computation
- Three saving throw types (Physical, Evasion, Mental)
- Weapon attack and damage bonus calculations
- Shock damage mechanics with AC thresholds
- Armor piercing weapon support
- Conditional weapon configuration (skill and attribute assignment)

#### Equipment & Inventory
- Multiple item types (General Item, Weapon, Armor)
- Equipment slot management (Stowed, Readied, Equipped)
- Individual item encumbrance tracking
- Weapon tags and properties
- Armor AC bonuses and shield support
- Item editing and deletion

#### Character Abilities
- Class abilities with automatic level-based filtering
- Multiple ability support per class
- Level requirement tracking for abilities

#### Foci System
- Multiple foci per character
- Foci effects with various types (bonuses, save improvements, etc.)
- Conditional foci activation
- Effect bonuses integrated into combat calculations
- Level-based focus management

#### Spellcasting
- Spell database management
- Known spells tracking
- Spell slot system (6 spell levels)
- Spell slot usage tracking
- Learn/forget spell mechanics

#### Quality of Life
- Character list with quick status summary
- Hit point tracker with +/- buttons
- Hit die display with modifiers
- Notes section for campaign information
- Edit mode for detailed character modifications

### Missing Features

#### High Priority
- **System Strain Tracking** - Core WWN mechanic for non-magical healing penalties
  - No max strain tracking (should equal CON score)
  - No current strain display
  - No strain recovery mechanics for rest

#### Medium Priority
- **Total Encumbrance Display**
  - No total carrying weight calculation
  - No max capacity display (based on STR score)
  - No visual encumbrance status warnings
  - No distinction between slot-specific encumbrance impact

- **Attack Roll Bonus Breakdown**
  - No detailed formula display for attack bonuses
  - No clear breakdown showing (attribute mod + skill mod + BAB + item bonuses)
  - Limited visibility into modifier composition

- **Initiative Mechanics**
  - No initiative bonus calculation
  - No combat initiative tracking

#### Low Priority
- **Proficiency/Skill Point Economy**
  - No tracking of available vs. spent skill points
  - No Expert class skill point restrictions enforcement
  - No attribute improvement tracking via skill points

- **Wealth/Treasure Tracking**
  - No separate money/treasure inventory
  - No wealth management for hireling payments

- **Expanded Combat Display**
  - Limited shock damage threshold visibility
  - Could enhance armor piercing weapon indication

- **Attribute-to-Skill Relationships**
  - No visual indicators of which attributes apply to skills
  - Could improve clarity of modified skill checks

## Technology Stack

### Frontend
- React + TypeScript
- Vite build tool
- ESLint for code quality

### Backend
- .NET
- Domain-Driven Design architecture
- Entity Framework Core

## Getting Started

### Prerequisites
- Node.js 18+
- .NET 8+

### Installation

```bash
# Install frontend dependencies
cd src/WWN.Web/client-app
npm install

# Install backend dependencies
cd ../../..
dotnet restore
```

### Development

```bash
# Start frontend dev server
cd src/WWN.Web/client-app
npm run dev

# Start backend
cd ..
dotnet run
```

## Project Structure

```
src/
├── WWN.Domain/          # Core domain models and rules
├── WWN.Application/     # Application services
└── WWN.Web/
    ├── Program.cs       # API configuration
    └── client-app/      # React frontend
        └── src/
            ├── components/  # UI components
            ├── pages/      # Page layouts
            ├── hooks/      # Custom React hooks
            ├── api/        # API client
            └── types/      # TypeScript types
```

## Contributing

Contributions are welcome! Please ensure:
- Code follows the existing style
- Features are tested
- Tests pass before submission

## License

See LICENSE file for details.
