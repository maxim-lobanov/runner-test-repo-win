param(
   [string] $WriteString
)

if ($WriteString -ne 'hello'){
    throw 'Failed to pass in right args'
}