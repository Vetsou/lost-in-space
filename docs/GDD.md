# Lost In Space
## 1. Summary

The player controls a character on a grid, moving one tile per step (UP, DOWN, LEFT, RIGHT).
The goal of each level is to collect all points on the map, then reach the "finish" tile.
The game will consist of 100 levels. Players can freely replay any previously completed level, and the UI will provide clear progress tracking.
In addition to completing each level, players are challenged to finish in the fewest steps possible.

- **Title**: "Lost In Space" (May change later),
- **Platform**: PC,
- **Target Audience**: All ages, all genders; fans of logic, puzzle, and optimization games,
- **Genre**: Grid-based puzzle,
- **Core Objective**: Collect all mandatory points on the map, then reach the finish tile,
- **Additional Objective**: Collect additional points on the map, finish level in the fewest steps possible,
- **Game Structure**: 30–100 hand-crafted levels; players can replay any completed level freely,
- **Progression Tracking**: UI displays step count, personal bests, collectible status, and level completion ratings,
- **Key Mechanics**: Grid-based movement; interactive platforms (teleporters, destructible tiles, directional tiles); buttons and switches,
- **Visual Style**: 3D low-poly with pixel-art UI, simple, clear,
- **Tech Stack**: Godot (C#); shaders; SteamAPI for achievements and Workshop integration

## 2. Game overview

### Core Loop
The player experience follows a clear, repeatable cycle:
- Select Level – Player chooses a level from the Level Select screen, either progressing linearly or replaying a previously completed stage,
- Navigate Level Grid – Player moves their character across the tile-based map, collecting points while avoiding or utilizing various platform types and interactive elements,
- Collect All Points – All mandatory points must be gathered before the finish tile becomes accessible or relevant.
- Reach Finish – Player steps onto the finish tile to complete the level,
- Review Performance – A completion screen displays step count, collectibles gathered, and a rating (e.g., 1–3 stars) based on efficiency and/or completion percentage,
- Replay or Advance – Player can proceed to the next level, replay the current level to improve their step count or collect remaining optional items, or return to Level Select

### Visual Style

- **3D Low-Poly** - Simple 3D models with simple shading and textures. UI elements may utilize pixel art to create a stylistic contrast.

Players must be able to quickly distinguish:
- Mandatory points from optional collectibles
- Different platform types (destructible, directional, teleporters, etc.)
- Interactive elements (buttons, switches, pressure plates)
- Player position

## 3. Gameplay Mechanics

### Movement
The player controls a character on a grid-based map. Movement is 'turn-based' and restricted to four cardinal directions: UP, DOWN, LEFT, RIGHT. Each input moves the character exactly one tile, unless blocked by an obstacle or level boundary.

### Collectibles
Collectibles are the primary objective of each level. They are divided into two categories:
- **Mandatory Points** - Must collect all to complete the level. The finish tile may be accessible earlier, but the level cannot be completed until all mandatory points are collected.
- **Optional Collectibles** – Not required to complete a level. Collecting all optional items contributes to 100% completion and may unlock achievements. To increase the challenge, some collectibles may have a step counter; once it reaches zero, the item becomes permanently unavailable for that attempt.

### Platform Types
Platforms are special tiles that modify movement, player state, or level conditions. Each platform type has a distinct visual appearance to ensure readability.

| Platform | Behavior |
|---|---|
| Standard Tile | Default walkable tile. No special effect. |
| Directional Tile | When stepped on, propels the player one tile in a specified direction (arrow). |
| Slippery Platform | When stepped on, the player continues moving in the same direction for an additional tile. |
| Destructible Tile | Breaks or disappears after being stepped on once. |
| Durable Destructible Tile | Breaks or disappears after being stepped on three times. |
| Teleporter | Transports the player to a corresponding teleporter tile elsewhere on the map. Should be bidirectional. Visual pairing (e.g., matching colors) indicates connections. |
| Finish Tile | Ends the level when stepped on, but only if all mandatory points have been collected. If mandatory points remain, the tile does nothing. |

### Interactive Elements
Interactive elements respond to player actions (stepping, pressing, or triggering) and alter the level state.

| Element | Behavior |
|---|---|
| Button | Activated when stepped on. Can trigger blocking tiles, bridges, or other level elements. Should toggle one-time. |
| Blocking Tile | Impassable. May be unlocked via buttons/switches, or other conditions. |

### Level Completion Conditions
A level is considered completed when:
1. All mandatory points have been collected
2. The player steps onto the finish tile

Optional collectibles do not affect level completion but are tracked separately for 100% completion.

## 4. Characters and World

### Player character
A lone space explorer whose ship crash-landed on an unknown, abandoned planet. Their only goal is to repair the ship to escape.

### World
The game takes place across the crash site and its surrounding environment. The planet is desolate but not empty; remnants of unknown technology and strange energy sources are scattered throughout.

### Progression Context
- Mandatory points - scrap, minerals, or repair components.
- Optional collectibles - rare alloys, data logs, or alien artifacts.

## 5. UI
The user interface should prioritize **minimalism**, **clarity**, and **accessibility**. The UI should not clutter the screen and only display the necessary information. All UI elements should follow a consistent color scheme and style. The preferred visual style for UI elements is pixel art.

### Main Menu
The main menu serves as the primary entry point to the game. It features the game's title logo prominently and presents the following options:

| Option | Description |
|---|---|
| Play | Navigates to the Level Select screen. |
| Level Creator | Opens the in-game level editor, allowing players to design and test custom levels. |
| Workshop | Opens the Steam Workshop interface for browsing, downloading, and rating community-created levels. |
| Achievements | Displays the player's unlocked achievements and progress toward locked ones. |
| Settings | Opens the settings menu (audio, video, controls, etc.). |
| Exit | Closes the game. |

Additional elements: Background animation or ambient visuals, version number, and credits accessible via a small button or footer.

### Level Select
The Level Select screen uses a grid layout to display available levels. Levels may be organized into worlds or chapters, each representing a set of levels grouped by theme or difficulty.

| Element | Description |
|---|---|
| Level Number | Clearly displayed (e.g., "1-1", "1-2"). |
| Rating | 1–3 star rating based on step count efficiency and/or optional collectible completion. |
| Lock State | Locked levels are visually distinct (padlock icon, chains, or darkened tile) and cannot be selected until the previous level is completed. |
| 100% Completion Indicator | A small icon (e.g. golden star or checkmark) appears on levels where all mandatory points and optional collectibles have been collected. |
| Thumbnail | Optional small preview image showing the level layout or theme. |


### In-Game UI
During gameplay, essential information is displayed in a clean top bar. UI elements are semi-transparent or positioned to minimize obstruction of the game grid.

| Element | Description |
|---|---|
| Level Number | Current level identifier (e.g., "Level 1-3"). |
| Step Counter | Current number of steps taken in this attempt. |
| Best Steps | Personal best step count for this level (displayed only if a record exists). |
| Points Collected | Counter showing mandatory points collected / total mandatory points (e.g., "5 / 8"). |
| Optional Collectibles | Counter showing optional collectibles collected / total optional collectibles (if present in the level). |
| Reset Button | Resets the level to its initial state; resets step counter, respawns all platforms and all collectibles. |
| Menu Button | Pauses the game and opens the pause menu (Resume, Level Select, Settings, Exit to Main Menu) |

### Pause Menu
The pause menu appears when the player presses the Menu button or the Escape key during gameplay. Options include:

| Option | Description |
|---|---|
| Resume | Returns to gameplay. |
| Restart | Resets the current level (same as Reset button). |
| Level Select | Exits the level and returns to Level Select screen. |
| Settings | Opens settings menu. |
| Exit to Main Menu | Returns to Main Menu. |

### Level Completion Screen
After the player reaches the finish tile with all mandatory points collected, the Level Completion Screen appears.

#### Displayed Information
| Element | Description |
|---|---|
| Steps Taken | Total steps used to complete the level. |
| Best Steps | Personal best step count (if this attempt is a new record, highlight it). |
| Collectibles Collected | Optional collectibles gathered / total optional collectibles. |
| Rating | 1–3 star rating based on step efficiency and/or optional collectible completion. |
| 100% Indicator | Special icon or message if all mandatory points and optional collectibles were collected. |

Rating criteria should be as follows:
- 1 star - Level was completed,
- 2 stars - Collected all optional items/Completed level in optimal step count,
- 3 stars - Collected all optional items and in optimal step count (can be from separate runs).

#### Action Buttons
| Button | Description |
|---|---|
| Next Level | Advances to the next unlocked level (if available). |
| Replay | Restarts the current level for another attempt. |
| Level Select | Returns to Level Select screen. |

### Level Creator
The Level Creator provides tools for players to design custom levels.
_TODO: It's hard to define what we need right now._

### Workshop
Allows to download custom levels from other players using steam workshop.
_TODO: It's hard to define what we need right now._

### Achievements
Achievements menu allows player to see a grid list of all achivements the game has. This achivements should
be displayed in gray color if not unlocked and normal color if achived.

### Settings Menu
Settings are accessible from both the Main Menu and the Pause Menu. Settings persist between game sessions.

| Category | Options |
|---|---|
| Audio | Master Volume, Music Volume, SFX Volume |
| Video | Fullscreen toggle, Resolution, V-Sync, Graphics Quality preset |
| Controls | Keybindings display and remapping |
| Gameplay | Language select, gameplay hints |

## 6. Requirements

### Art Assets

| Category | Assets | Description |
|---|---|---|
| UI | Title logo | Game logo displayed on Main Menu and loading screens |
| UI | Icon set (menu) | Pixel art of menu (hamburger/pause), settings cog, rest (circular arrow) |
| UI | Icon set (gameplay) | Pixel art of collectible (scrap metal), optional collectible (crystal/data drive) |
| UI | Button | Generic button texture for menus |
| UI | Font | Pixel font for text |
| UI | Rating starts | 1–3 star icons for level completion (both filled and hollow versions) |
| Character | Player model | Low-poly space explorer/survivor |
| Character | Player animations | Idling animation, move animation, victory animation |
| Character | Player VFX | Step dust effect, teleport flash, collectible pickup |
| Environment | Grid tiles | Standard platform, directional platform, ice/slippery platform, destructible platform (durable with 3 stages), teleporter, finish tile |
| Environment | Interactive elements | Button (with 2 states), blocking tile (rubble and force field with different colors), bridge tile (2 states on/off) |
| Environment | Collectibles | Mandatory point (scrap, gear etc.), optimal collectible (alien artifact, crystal, data drive) |
| Environment | Environmental props | Low-poly rocks, alien flora, wreckage pieces scattered and space ship model |

### Audio Assets

| Category | Assets | Description |
|---|---|---|
| Music | Menu music | Calm, looping track for Main Menu and settings screens |
| Music | Level completion | Short 3–5 second uplifting melody |
| SFX | Movement | Footstep sounds for default platforms |
| SFX | Collectible pickup | Sound for picking up a collectible |
| SFX | Teleport | Sound after stepping on teleport |
| SFX | Button press | Mechanical click for button press |
| SFX | Durable destructible tile crack | Cracked sound when player steps off durable platform |
| SFX | Destructible tile break | Shatter sound when destructible platform is destroyed |
| SFX | Directional tile push | Short sound when directional platform pushes player |
| SFX | UI confirm | Click sound for menu buttons |
| SFX | Level reset | Rewind / reverse sound effect |

### Technical Requirements

| Category | Requirements |
|---|---|
| Game Engine | Godot (C#) |
| Graphics | Custom shader support for visual effects |
| Platform Support | Windows/Linux; optional web app |
| Input | Keyboard and mouse support; optional controller support |
| Steam Integration | SteamAPI for achievements; Steam Workshop for level sharing and downloading |
| Save Data | Local save files for level progress, step records, 100% completion status, and settings |
| Level Editor | In-Game level editior that allows to export/import files; Workshop publishing support |