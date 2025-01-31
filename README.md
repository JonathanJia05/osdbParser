# OsdbParser

**OsdbParser** is a a tool I created for parsing `.osdb` files created by [Piotrekol's CollectionManager](https://github.com/Piotrekol/CollectionManager) (It's basically a clone of the read logic). It extracts collection and beatmap data and exports the results to a JSON file to be used in other applications.

---

## Features

- Parse `.osdb` files to extract collection and beatmap info.
- Export parsed data into a JSON file.

---

## Requirements

- .NET 6.0 SDK or later
- `.osdb` file (created by CollectionManager or from Osu!Stats etc.)

---

## Setup

### 1. Clone repo

```bash
git clone https://github.com/JonathanJia05/osdbParser.git
cd osdbParser
```

### 2. Install Dependencies

```bash
dotnet restore
```

---

## Usage

Run the project by:

```bash
dotnet run --project osdbParser -- <input.osdb> <output.json>
```

### Arguments

```bash
1. `<input.osdb>`: Path to the `.osdb` file you want to parse.
2. `<output.json>`: Path where the parsed JSON file will be saved.
```

### Example

```bash
dotnet run --project osdbParser -- "examples/sample.osdb" "output/collections.json"
```

---

## JSON Output Example

```json
[
  {
    "Name": "Collection name",
    "Editor": "Collection editor",
    "Beatmaps": [
      {
        "MapId": 123456,
        "ArtistRoman": "Artist Name",
        "TitleRoman": "Song Title",
        "DiffName": "Difficulty Name",
        "Md5": "md5hash12345",
        "StarsNomod": 5.24
      },
      {
        "MapId": 67890,
        "ArtistRoman": "Artist Name",
        "TitleRoman": "Song Title",
        "DiffName": "Difficulty Name",
        "Md5": "md5hash67890",
        "StarsNomod": 1.4
      },
      ....
    ]
  }
]
```

---

## Acknowledgements

This tool is just a modification and simplification of [Piotrekol's CollectionManager](https://github.com/Piotrekol/CollectionManager).

---

Feel free to modify or extend this as needed!
