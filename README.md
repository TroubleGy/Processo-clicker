# Processo-clicker

Process-focused auto clicker for Windows.  
Attaches to a specific window and sends clicks **only there**.  
No interference with background apps, no global chaos — just clean execution.

---

## Features

- Manual window selection
- Clicks strictly inside attached process
- Hotkey to start/stop (F6)
- Interval configuration in milliseconds
- Visual feedback
- Smooth UI with custom gradient controls
- Acrylic blur and rounded corners
- Doesn't block UI or mouse movement

---

## How it works

1. **Launch ProcessoClicker**
2. **Select a target window** from the dropdown
3. Click **Attach**
4. Point your cursor where you want the click to happen
5. Press **F6** to start clicking
6. Press **F6 again** to stop

You can clear the state using the **Clear** button.  
The app won't touch other windows or processes.

---

## Hotkeys

| Key | Action             |
|-----|--------------------|
| F6  | Start/Stop clicking |

---

## Build

Requirements:
- .NET 9.0 (can work with 6.0+, just update target)
- Windows OS
- Visual Studio or `dotnet CLI`

Steps:

```bash
git clone https://github.com/TroubleGy/processo-clicker.git
cd processo-clicker
```
Then open `ProcessingClicker.sln` in Visual Studio

or build via:
```bash
dotnet build
```
---

## License
- This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).  
- Do whatever you want with it, but keep it clean.

---

## Notes
- Clicks are sent via WinAPI (SendMessage), not simulated via actual input.
- This keeps behavior limited to the window only.
- Perfect for use-cases where full control is needed without affecting your system.

---

## Credits
- Built and maintained by [TroubleGy](https://github.com/TroubleGy).
- Made for those who like precise tools with no BS.
