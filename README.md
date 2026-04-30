# SC Češtinátor (Avalonia)

![Build](https://github.com/1walkerit/sc-cestinator-avalonia/actions/workflows/build-appimage.yml/badge.svg)
![Release](https://img.shields.io/github/v/release/1walkerit/sc-cestinator-avalonia)
![Downloads](https://img.shields.io/github/downloads/1walkerit/sc-cestinator-avalonia/total)

Linux GUI nástroj pro instalaci, správu a údržbu české lokalizace pro **Star Citizen**.

Moderní rewrite původní Python verze do **C# (.NET 8) + Avalonia UI**.

---

## 📦 Stažení

👉 **[Stáhnout nejnovější AppImage](https://github.com/1walkerit/sc-cestinator-avalonia/releases/latest)**

---

## ✨ Funkce

### 🎮 Lokalizace

* instalace české lokalizace
* aktualizace na nejnovější verzi
* odinstalace (se zálohou originálních souborů)
* detekce lokální vs online verze

### 🧰 Nástroje

* vymazání shader cache (`~/.cache/mesa_shader_cache`, `~/.cache/nvidia`)
* vyčištění logů (LIVE / PTU)
* otevření game složky
* uložení poslední použité cesty

### 🔄 Aktualizace aplikace

* kontrola nové verze (GitHub API)
* stažení nové verze
* progress bar při downloadu
* otevření složky po stažení

---

## 🖼️ Screenshot

<img width="1243" height="716" src="https://github.com/user-attachments/assets/80d2928b-00ea-4486-bac0-ee9ee6959174" />

---

## 🚀 Spuštění (AppImage)

```bash
chmod +x SC-Cestinator-*.AppImage
./SC-Cestinator-*.AppImage
```

---

## 🧪 Spuštění ze zdrojového kódu

### Požadavky

* .NET 8 SDK
* Linux

### Build + run

```bash
dotnet build
dotnet run
```

---

## 🏗️ Technologie

* C# (.NET 8)
* Avalonia UI
* CommunityToolkit.Mvvm
* GitHub Actions (build AppImage)

---

## 📁 Struktura projektu

```
Services/      → logika (GitHub, FS, dialogy)
ViewModels/    → MVVM
Views/         → UI (Avalonia XAML)
Assets/        → ikony, obrázky
AppDir/        → AppImage struktura
```

---

## 🔗 Původní projekt

Python verze:
https://github.com/1walkerit/sc-cestinator-linux

---

## ⚠️ Poznámky

* aplikace je určena pro Linux (Wine / Lutris kompatibilní)
* testováno primárně na Arch Linux
* používání na vlastní riziko (zásah do herních souborů)
