#!/bin/bash
# Navigate to your repository's directory
# cd /path/to/your/repository
# Initialize Git LFS if not already done
# git lfs install

echo "Checking for files larger than 10MB and not ignored by .gitignore..."
# Find files larger than 10MB excluding the .git directory
find . -type f -size +10M ! -path './.git/*' | while read -r file
do
    # Check if the file is ignored by .gitignore
    if ! git check-ignore -q "$file"; then
        # Check if the file is already tracked in .gitattributes
        if ! grep -q "$(basename "$file")" .gitattributes; then
            git lfs track "$file"
            echo "Tracking $file with Git LFS"
        else
            echo "Skipping already tracked file $file"
        fi
    else
        echo "Skipping ignored file $file"
    fi
done

# Add the .gitattributes file to Git if changes were made
git add .gitattributes
#git diff-index --cached --quiet HEAD -- || git commit -m "Configure Git LFS tracking for large files"