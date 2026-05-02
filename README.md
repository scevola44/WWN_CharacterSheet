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

## Self-Hosting

This application can be self-hosted using several methods. Choose the option that best fits your infrastructure.

### Local/Manual Setup

For hosting on your own server without Docker:

#### Prerequisites
- .NET SDK 10.0 or later
- Node.js 20 or later
- A system with at least 512MB RAM

#### Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/scevola44/WWN_CharacterSheet.git
   cd WWN_CharacterSheet
   ```

2. **Build the frontend:**
   ```bash
   cd src/WWN.Web/client-app
   npm install
   npm run build
   cd ../../..
   ```

3. **Restore backend dependencies:**
   ```bash
   dotnet restore
   ```

4. **Run the application:**
   ```bash
   dotnet run --configuration Release
   ```

5. **Access the application:**
   - Open your browser and navigate to `http://localhost:5000`
   - The SQLite database will be created automatically at `wwn_characters.db` in the application root directory

#### Configuration

- **Port:** By default, the application runs on port 5000. Change it with the environment variable:
  ```bash
  ASPNETCORE_URLS=http://0.0.0.0:8080 dotnet run --configuration Release
  ```

- **Environment:** Set to production mode automatically when using `--configuration Release`

- **Database:** SQLite database is stored locally as `wwn_characters.db`. Ensure the application has write permissions in its directory.

### Docker Deployment

For containerized self-hosting, we support both Docker Compose and manual Docker setup.

#### Prerequisites
- Docker and Docker Compose installed and running

#### Docker Compose (Recommended)

The easiest way to run the application with Docker:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/scevola44/WWN_CharacterSheet.git
   cd WWN_CharacterSheet
   ```

2. **Start the application:**
   ```bash
   docker-compose up -d
   ```
   This will automatically build the image and start the container.

3. **Access the application:**
   - Open your browser and navigate to `http://localhost:5000`

4. **View logs:**
   ```bash
   docker-compose logs -f
   ```

#### Docker Compose Commands

- **Stop the application (keeps data):**
  ```bash
  docker-compose stop
  ```

- **Start the application:**
  ```bash
  docker-compose start
  ```

- **Restart the application:**
  ```bash
  docker-compose restart
  ```

- **Stop and remove containers (keeps data volumes):**
  ```bash
  docker-compose down
  ```

- **Stop and remove everything including data volumes:**
  ```bash
  docker-compose down -v
  ```

#### Customizing Environment Variables

To modify the port or other settings, create a `.env` file in the same directory as `docker-compose.yml`:

```env
ASPNETCORE_URLS=http://+:8080
```

Then run:
```bash
docker-compose up -d
```

#### Manual Docker (Alternative)

If you prefer to use Docker without Compose:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/scevola44/WWN_CharacterSheet.git
   cd WWN_CharacterSheet
   ```

2. **Build the Docker image:**
   ```bash
   docker build -t wwn-charactersheet .
   ```

3. **Run the container:**
   ```bash
   docker run -d \
     --name wwn-charactersheet \
     -p 5000:5000 \
     -v wwn-data:/data \
     wwn-charactersheet
   ```

4. **Access the application:**
   - Open your browser and navigate to `http://localhost:5000`

5. **View logs:**
   ```bash
   docker logs wwn-charactersheet
   ```

6. **Stop the container:**
   ```bash
   docker stop wwn-charactersheet
   ```

7. **Start the container:**
   ```bash
   docker start wwn-charactersheet
   ```

8. **Remove the container (keeps data):**
   ```bash
   docker rm wwn-charactersheet
   ```

#### Data Persistence

The application uses a Docker volume to persist your character data:
- Volume name: `wwn-data`
- Mount point: `/data` inside the container
- Contains: SQLite database and application logs

To view volumes:
```bash
docker volume ls | grep wwn-data
```

### Fly.io Deployment

For hosting on Fly.io:

#### Prerequisites
- Fly.io account ([sign up here](https://fly.io))
- `flyctl` CLI installed

#### Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/scevola44/WWN_CharacterSheet.git
   cd WWN_CharacterSheet
   ```

2. **Create a Fly.io app:**
   ```bash
   flyctl launch
   ```
   - The project includes a `fly.toml` configuration file that will be used automatically
   - Follow the prompts to set up your app name and region

3. **Deploy:**
   ```bash
   flyctl deploy
   ```

4. **Access your application:**
   ```bash
   flyctl open
   ```

The configuration in `fly.toml` includes:
- Persistent volume for data storage
- Automatic scaling (minimum 0 machines, auto-start enabled)
- Health check endpoint configured
- Runs on port 5000 internally

### Render Deployment

For hosting on Render:

#### Prerequisites
- Render account ([sign up here](https://render.com))
- GitHub repository with the code

#### Steps

1. **Fork or push this repository to GitHub**

2. **Create a new Web Service on Render:**
   - Go to [Render Dashboard](https://dashboard.render.com)
   - Click "New +" and select "Web Service"
   - Connect your GitHub repository
   - Choose the branch to deploy (defaults to `develop`)

3. **Render will automatically use the `render.yaml` configuration:**
   - The project includes a `render.yaml` blueprint that defines the deployment
   - Docker build will be triggered automatically
   - Service will be exposed on the provided URL

4. **Access your application:**
   - Your app will be available at the URL provided by Render (typically `https://your-app-name.onrender.com`)

The configuration in `render.yaml` includes:
- Docker containerization
- Automatic deployment from specified branch
- Health check endpoint
- Runs on port 5000 internally

## Configuration

### Environment Variables

The application respects the following environment variables:

- **ASPNETCORE_ENVIRONMENT:** Set to `Production` for production deployments (default: `Development`)
- **ASPNETCORE_URLS:** Configure the listening URL (default: `http://0.0.0.0:5000`)
- **ConnectionStrings__DefaultConnection:** SQLite database path (default: `./wwn_characters.db`)

### Port Configuration

- Default port: `5000`
- For Docker: Map host port to container port 5000 using `-p HOST_PORT:5000`
- For custom ports: Set `ASPNETCORE_URLS` environment variable

### Data Persistence

- **Local/Manual:** Database stored as `wwn_characters.db` in the application root
- **Docker:** Database stored in Docker volume at `/data/wwn_characters.db`
- **Fly.io:** Database stored in persistent volume configured in `fly.toml`
- **Render:** Database stored in ephemeral storage (note: data persists during deployments but resets when services restart)

## Troubleshooting

### Port Already in Use
If port 5000 is already in use:
- **Local:** Change the port using `ASPNETCORE_URLS=http://0.0.0.0:8080 dotnet run`
- **Docker:** Use a different host port: `docker run -p 8080:5000 wwn-charactersheet`

### Database Connection Issues
- Ensure the application has write permissions in its directory
- Check that the `wwn_characters.db` file exists and is readable
- For Docker, verify the volume is properly mounted: `docker inspect wwn-charactersheet`

### Frontend and Backend Connection Problems
- Verify the backend is running and accessible at `http://localhost:5000`
- Check CORS configuration if running frontend and backend on different hosts
- Review logs for any error messages: `docker logs wwn-charactersheet` (for Docker)

### Application Won't Start
- Check that the correct .NET SDK version is installed: `dotnet --version`
- Verify Node.js version for manual builds: `node --version`
- Check available disk space and memory

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
