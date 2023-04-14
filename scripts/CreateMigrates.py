"""
TheNexusAvenger

Helper script for creating database migrates.
"""

import re
import os
import subprocess
from typing import Callable


def createMigrate(contextName: str, migrationName: str) -> None:
    """Creates a database migrate for a database context.

    :param contextName: Name of the database context file.
    :param migrationName: Name of the database migrate to create.
    """

    # Start the dotnet build.
    databaseDirectory = os.path.realpath(os.path.join(__file__, "..", "..", "Nexus.Clearing.Server"))
    process = subprocess.Popen(["dotnet", "ef", "migrations", "add", migrationName, "--context", contextName], cwd=databaseDirectory)

    # Wait for the build to complete and throw an exception if it failed (exit code is not 0).
    process.wait()
    if process.returncode != 0:
        raise Exception("Migrate add process failed with exist code " + str(process.returncode))


if __name__ == '__main__':
    import sys

    # Get the database migrate name.
    if len(sys.argv) < 2:
        print("Usage: CreateMigrates.py migrationName")
        exit(-1)
    migrationName = sys.argv[1]

    # Create the database migrate.
    createMigrate("SqliteContext", "Sqlite" + migrationName)