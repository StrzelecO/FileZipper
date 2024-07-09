
# File Zip Application

## Overview

This repository contains a simple C# console application created as one of the initial tasks during my internship. The application is designed to zip files based on parameters specified in the `App.config` file and logs information about each operation to a file.

### Parameters

The application uses the following parameters:

- **SourceFolder**: The folder from which files will be zipped.
- **DestinationFolder**: The folder where the .zip files will be saved.
- **Template**: A template specifying which files to zip (e.g., only .txt files).
- **Days**: Specifies how old (in days) files should be to be included in the zip operation.
- **ZippedFileName**: Allows specifying a template name for the zipped files.
