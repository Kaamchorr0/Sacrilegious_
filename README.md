📌 **Project Overview**

This project is a real-time **2D boss battle game** developed in **Unity**. The core feature is a dynamic boss that utilizes a **Generative AI (Google's Gemini API)** to adapt to the player's combat style. The AI "learns" from the player's previous attempts to generate unique, counter-strategy attack patterns, creating a new challenge with every respawn.

🛠️ **Features**

* **Adaptive Boss AI:** The boss's "Brain" (`Tactics.cs`) analyzes player attack data to generate new strategies.
* **Dynamic Attack Patterns:** Utilizes the **Gemini API** to procedurally generate a 10-step attack sequence.
* **Player Learning Loop:** A "Souls-like" death/respawn system (managed by `Phealth.cs`) acts as the trigger for the AI's learning process.
* **Responsive 2D Combat:** Full-featured player controller with melee, long-range, and jump mechanics.
* **Complex Boss Moveset:** The boss "Body" (`BossController.cs`) features melee, dashing, guarding, and healing abilities.

🚀 **System Workflow**

1. **Player Fights:** The player uses melee and long-range attacks to fight the boss.
2. **"Spy" Script (`AttackTracker.cs`):** Logs every *successful* player hit to `PlayerPrefs`, sorting them by "Close" or "Long" range.
3. **Player Dies:** `Phealth.cs` detects player death. It teleports the player back to the respawn point and calls the boss's reset functions.
4. **Boss "Learns":** `Phealth.cs` calls `Tactics.ResetAndRethink()`. The "Brain" script stops, reads the `PlayerPrefs` data (e.g., "10 melee hits, 2 ranged hits"), and builds a prompt (e.g., "Player is a brawler, counter them").
5. **Gen AI Call:** The `Tactics.cs` script sends this prompt to the **Google AI (Gemini) API** while a loading screen is shown (to hide the 2-3 second API lag).
6. **New Plan:** The API returns a new 10-step attack plan (e.g., `Dash,Guard,Melee,Dash...`).
7. **Fight!:** The `Tactics.cs` script (Brain) begins executing the new plan, telling the `BossController.cs` (Body) which moves to perform. The loop repeats.

🔧 **Technical Requirements**

**Hardware**

* A PC or Mac capable of running the Unity Editor.

**Software**

* **Unity Hub** & **Unity 2022.x** (or newer).
* **VS Code** (for C# development).
* **Google AI Studio API Key:** This is **required** for the AI to function.
* **Active Internet Connection:** Required at runtime for the API calls.

📌 **Deployment Guide**

1️⃣ **Clone the Project**

* Clone this repository to your local machine.
* Open the project in **Unity Hub**.

2️⃣ **Configure the AI API Key**

1. Go to **Google AI Studio** and generate a new, free API key.
2. In Unity, find the **`Boss`** GameObject in the Hierarchy.
3. Select it and find the **`Tactics (Script)`** component in the Inspector.
4. Paste your API key into the public **`API_KEY`** field.

3️⃣ **Launch the Game**

* Press the **Play** button in the Unity Editor.
* Teleport to the boss arena to trigger the first API call and start the fight.

📝 **Code Overview**

* The **`Tactics.cs`** (Brain) script manages the "learning" loop. It reads `PlayerPrefs`, builds the AI prompt, calls the API, and executes the returned plan by calling functions on the `BossController`.
* The **`BossController.cs`** (Body) script is the "muscle." It contains all the boss's abilities (`DoDash`, `DoMeleeAtk`, `DoGuard`, `DoHeal`) as public coroutines and functions. It also manages its own health, states (`isBusy`, `isGuarding`), and animations.
* The **`Phealth.cs`** (Player Health) script manages the player's life. Its `Die()` function is the "quarterback" of the entire reset loop, telling both the player and the boss to reset.
* The **`AttackTracker.cs`** (Spy) script is attached to the player and provides the intelligence for the AI by logging successful hits.
* The **`Movement.cs`** (Player Controller) script handles all player input, movement, and attack logic.

📈 **Future Enhancements**

🔹 **Enhanced AI Vocabulary:** Add more moves (e.g., summons, AOE attacks) to the `BossController` and the AI's prompt.
🔹 **Visual Feedback:** Implement a "You Win" screen and visual particle effects for healing/dashing.
🔹 **Refined Learning:** Have the AI analyze *which* boss moves were successfully dodged or blocked, not just the player's attacks.
🔹 **Difficulty Scaling:** Increase boss speed or damage after each successful player respawn.
🔹 **Sound Design:** Add sound effects for attacks, guards, and the boss's generative taunts (if added).

🌍 **Application Scope**

* **Game Jams & Competitions:** A perfect example of a high-concept, "wow-factor" project that is finishable in a short time.
* **AI in Games Prototyping:** A clean framework for testing API-driven game logic and dynamic difficulty.
* **Educational Projects:** Demonstrates how to integrate external, web-based AI into a real-time Unity application.

📜 **License**

This software and its associated code are **proprietary**. Unauthorized reproduction, modification, or distribution is strictly prohibited without prior written consent from the author.
