#!/bin/bash
dotnet DungeonCrawler.Editor/bin/Debug/net8.0/DungeonCrawler.Editor.dll -b "PixelDungeon" -o ../DC\ Jam\ 2024\ Unity/Assets/Data/Manifests/Test.json
cp ../DC\ Jam\ 2024\ Unity/Assets/Data/Manifests/Test.json /d/tmp/project.json
cp ../DC\ Jam\ 2024\ Unity/Assets/Data/Manifests/Test.json ../Builds/WebGL/project.json