import re
import sys

def main():
    path = "c:/Users/hp/OneDrive/Desktop/pmc pvc card work flow/pmc_activation_app/lib/services/api_service.dart"
    with open(path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    out = []
    skip = False
    brace_depth = 0
    in_else = False
    
    i = 0
    while i < len(lines):
        line = lines[i]
        
        if "if (_isSimulated) {" in line:
            skip = True
            brace_depth = 1
            i += 1
            continue
            
        if skip:
            if "{" in line:
                brace_depth += line.count("{")
            if "}" in line:
                brace_depth -= line.count("}")
                
            if brace_depth == 0:
                # Reached end of if (_isSimulated)
                # Next line should be } else {
                skip = False
                in_else = True
                # Skip the } else { line
                if i + 1 < len(lines) and "} else {" in lines[i+1]:
                    i += 1
                elif "} else {" in line:
                    pass
            i += 1
            continue
            
        # If we were in_else, we need to find the matching closing brace for the else block.
        # But wait, it's easier to just strip `if (_isSimulated) { ... } else {` 
        # and then we have an extra `}` at the end of the block.
        out.append(line)
        i += 1
        
    with open(path, "w", encoding="utf-8") as f:
        f.writelines(out)

if __name__ == "__main__":
    main()
