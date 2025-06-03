# Data Communication Project

**IMPORTANT SETUP STEP**  
Before running, you must move the `inetpub` folder from this repository into your C:\ drive.

## How to configure

1. **Clone or download** this repository (you will see an `inetpub` folder here).  
2. **Copy** (or cut) the `inetpub` folder into `C:\inetpub`.  
   - Example (PowerShell):
     ```powershell
     # If you cloned to C:\Users\<name>\Projects\DataComm..., run:
     Copy-Item -Path "C:\Users\<name>\Projects\DataComm...\inetpub" -Destination "C:\inetpub" -Recurse
     ```
3. **Open** the solution in Visual Studio (inside `Template[2024-2025]/…`).  
4. **Build & Run** normally (F5). The code expects its web files under `C:\inetpub`.

> If you do not place `inetpub` under `C:\`, the application will fail to find its web content.

---

## Repository Contents

- `.vscode/`  
- `inetpub/`  
- `Template[2024-2025]/` (your VS solution and source code)  
- `HTTPProject 2025.pptx` (slides)  
- `README.md` (this file)

Nothing else is needed. Once `inetpub` is in `C:\inetpub`, your IIS or local webserver can serve from that path and the project will run.
