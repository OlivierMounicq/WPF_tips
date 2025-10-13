# Method 1: Basic loop to extract names

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

# Loop through each record

foreach ($record in $json) {
$firstName = $record.data[0].value  # First name is in data[0]
$lastName = $record.data[1].value   # Last name is in data[1]

```
Write-Host "First Name: $firstName, Last Name: $lastName"
```

}

# ============================================

# Method 2: Extract names and create objects

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

$results = @()

foreach ($record in $json) {
$firstName = $record.data[0].value
$lastName = $record.data[1].value

```
$nameObject = [PSCustomObject]@{
    FirstName = $firstName
    LastName = $lastName
    FullName = "$firstName $lastName"
}

$results += $nameObject
```

}

# Display results

$results | Format-Table -AutoSize

# ============================================

# Method 3: Export results to CSV

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

$results = @()

foreach ($record in $json) {
$firstName = $record.data[0].value
$lastName = $record.data[1].value

```
$results += [PSCustomObject]@{
    FirstName = $firstName
    LastName = $lastName
    FullName = "$firstName $lastName"
}
```

}

# Export to CSV

$results | Export-Csv -Path “C:\output\names.csv” -NoTypeInformation

# ============================================

# Method 4: Using ForEach-Object (pipeline approach)

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

$json | ForEach-Object {
[PSCustomObject]@{
FirstName = $*.data[0].value
LastName = $*.data[1].value
}
} | Format-Table -AutoSize

# ============================================

# Method 5: With error handling

$jsonFile = “C:\path\to\your\file.json”

try {
$json = Get-Content $jsonFile | ConvertFrom-Json

```
foreach ($record in $json) {
    try {
        $firstName = $record.data[0].value
        $lastName = $record.data[1].value
        
        if ($firstName -and $lastName) {
            Write-Host "Name: $firstName $lastName"
        } else {
            Write-Warning "Missing name data in record"
        }
    }
    catch {
        Write-Error "Error processing record: $_"
    }
}
```

}
catch {
Write-Error “Error reading JSON file: $_”
}

# ============================================

# Method 6: Save to a variable and use later

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

$names = @()

foreach ($record in $json) {
$names += @{
FirstName = $record.data[0].value
LastName = $record.data[1].value
}
}

# Use the names variable

$names | ForEach-Object {
Write-Host “$($*.FirstName) $($*.LastName)”
}

# ============================================

# Method 7: Filter by condition (e.g., specific last name)

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

foreach ($record in $json) {
$firstName = $record.data[0].value
$lastName = $record.data[1].value

```
# Only display if last name matches condition
if ($lastName -eq "Jack") {
    Write-Host "Found: $firstName $lastName"
}
```

}

# ============================================

# Method 8: Count occurrences of first/last names

$jsonFile = “C:\path\to\your\file.json”
$json = Get-Content $jsonFile | ConvertFrom-Json

$nameCount = @{}

foreach ($record in $json) {
$firstName = $record.data[0].value
$lastName = $record.data[1].value
$fullName = “$firstName $lastName”

```
if ($nameCount.ContainsKey($fullName)) {
    $nameCount[$fullName]++
} else {
    $nameCount[$fullName] = 1
}
```

}

# Display counts

$nameCount | Format-Table -AutoSize