#!/usr/bin/env python3

import argparse
import os
from pathlib import Path

projectName = 'Blog.Api'

parser = argparse.ArgumentParser()

parser.add_argument('cmd')

parser.add_argument('--name', dest='name', default='')
parser.add_argument('--from', dest='_from', default='')
parser.add_argument('--to', dest='_to', default='')

args = parser.parse_args()

projectDir = Path(__file__).resolve().parent.parent
os.chdir(projectDir)

if args.cmd == 'update-scheme':
    os.system(f"dotnet ef migrations add {args.name}", )
elif args.cmd == 'generate-script':
    migrationDir = os.path.join(projectDir, 'Migrations')

    if args._from == '' and args._to == '':
        filename = f'{projectName}.sql'
        filepath = os.path.join(migrationDir, filename)
        os.system(f"dotnet ef migrations script -o {filepath}")
    elif args._from != '' and args._to == '':
        filename = f'{projectName}_from_{args._from}.sql'
        filepath = os.path.join(migrationDir, filename)
        os.system(f"dotnet ef migrations script -o {filepath}")
    elif args._from != '' and args._to != '':
        filename = f'{projectName}_from_{args._from}.sql'
        filepath = os.path.join(migrationDir, filename)
        os.system(f"dotnet ef migrations script -o {filepath}")
