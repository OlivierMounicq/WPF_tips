You want to apply changes from one file to a different file (not the same file in different branches). Here are several approaches:

## Option 1: Cherry-pick with Path Modification

This requires manual intervention since Git can’t automatically apply changes from one file to another:

```bash
# Create a patch from the specific commit
git format-patch -1 <commit-hash> --stdout > changes.patch

# Edit the patch file to change the file paths
# Change: --- a/source-file.txt
# To:     --- a/target-file.txt
# Change: +++ b/source-file.txt  
# To:     +++ b/target-file.txt

# Apply the modified patch
git apply changes.patch
```

## Option 2: Manual Diff and Apply

```bash
# See what changed in the commit for the source file
git show <commit-hash> -- path/to/source-file.txt

# Manually apply those same changes to your target file
# Edit target-file.txt with the same modifications

# Commit the changes
git add path/to/target-file.txt
git commit -m "Apply changes from source-file commit <commit-hash>"
```

## Option 3: Extract and Apply Changes

```bash
# Extract the changes from the commit
git show <commit-hash>:path/to/source-file.txt > temp-new-version.txt
git show <commit-hash>^:path/to/source-file.txt > temp-old-version.txt

# Create a diff between old and new versions
diff -u temp-old-version.txt temp-new-version.txt > changes.diff

# Manually edit changes.diff to reference your target file
# Then apply it
patch path/to/target-file.txt < changes.diff

# Clean up
rm temp-new-version.txt temp-old-version.txt changes.diff
```

## Option 4: Script-Based Approach

If you need to do this regularly, create a script:

```bash
#!/bin/bash
# apply-commit-to-file.sh

COMMIT_HASH=$1
SOURCE_FILE=$2  
TARGET_FILE=$3

# Extract the changes
git show $COMMIT_HASH -- $SOURCE_FILE > /tmp/changes.txt

echo "Changes from commit $COMMIT_HASH in $SOURCE_FILE:"
echo "Please manually apply these changes to $TARGET_FILE"
cat /tmp/changes.txt
```

Usage: `./apply-commit-to-file.sh abc123 config-dev.txt config-prod.txt`

## Option 5: Interactive Approach

```bash
# Show the commit changes
git show <commit-hash> -- source-file.txt

# Open both files in your editor
code source-file.txt target-file.txt

# Manually copy the relevant changes from source to target
# Then commit
git add target-file.txt
git commit -m "Apply changes from source-file commit <commit-hash>"
```

## Example Workflow

Let’s say you have `config-dev.txt` and `config-prod.txt`:

```bash
# 1. Find the commit that modified config-dev.txt
git log --oneline -- config-dev.txt

# 2. View what changed in that commit
git show abc123 -- config-dev.txt

# 3. Apply similar changes to config-prod.txt
# (manually edit the file)

# 4. Commit the changes
git add config-prod.txt  
git commit -m "Apply dev config changes (from commit abc123) to prod"
```

**Note:** Git can’t automatically transfer changes between different files because it doesn’t know how to map the changes. You’ll need to manually review and apply the relevant modifications.

Which approach works best for your use case? What types of files are you working with?​​​​​​​​​​​​​​​​