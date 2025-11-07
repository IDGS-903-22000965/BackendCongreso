#!/usr/bin/env bash
set -o errexit

dotnet restore
dotnet publish -c Release -o out