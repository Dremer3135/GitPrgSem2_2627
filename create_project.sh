#!/usr/bin/env bash

set -e

if [ -z "$1" ]; then
    echo "Usage: $0 <folder_name> [project_name]"
    echo "Example: $0 19_MujProjekt"
    echo "         $0 19_MujProjekt MujProjekt"
    exit 1
fi

FOLDER_NAME="$1"

# If project name is not explicitly passed as second argument:
if [ -n "$2" ]; then
    PROJECT_NAME="$2"
else
    # Automatically strip leading digits and underscores (e.g. '19_MujProjekt' -> 'MujProjekt')
    # If no leading digits, PROJECT_NAME equals FOLDER_NAME
    PROJECT_NAME=$(echo "$FOLDER_NAME" | sed -E 's/^[0-9]+_//')
fi

# Determine script root directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET_DIR="$SCRIPT_DIR/$FOLDER_NAME"

if [ -d "$TARGET_DIR" ]; then
    echo "Error: Directory '$TARGET_DIR' already exists." >&2
    exit 1
fi

echo "🚀 Creating project '$PROJECT_NAME' in '$FOLDER_NAME'..."

mkdir -p "$TARGET_DIR"
cd "$TARGET_DIR"

# 1. Create Visual Studio Solution (.sln)
dotnet new sln -n "$PROJECT_NAME"

# 2. Create Console Project (.csproj) targeting .NET 8.0 with explicit Program.Main structure
dotnet new console -n "$PROJECT_NAME" -f net8.0 --use-program-main

# 3. Add Project to Solution
dotnet sln add "$PROJECT_NAME/$PROJECT_NAME.csproj"

# 4. Create boilerplate Program.cs with namespace and class structure
cat << 'EOF' > "$PROJECT_NAME/Program.cs"
namespace PROJECT_NAME_PLACEHOLDER;

internal class Program
{
    static void Main(string[] args)
    {
        // TODO: Start writing your code here
    }
}
EOF

# Replace placeholder with the actual project name
sed -i "s/PROJECT_NAME_PLACEHOLDER/$PROJECT_NAME/g" "$PROJECT_NAME/Program.cs"

echo ""
echo "✅ Project '$PROJECT_NAME' successfully created in '$FOLDER_NAME/'!"
echo "📄 Program.cs initialized with namespace and static void Main."
echo "👉 To run:   dotnet run --project \"$FOLDER_NAME/$PROJECT_NAME\""
echo "👉 To build: dotnet build \"$FOLDER_NAME/$PROJECT_NAME.sln\""
