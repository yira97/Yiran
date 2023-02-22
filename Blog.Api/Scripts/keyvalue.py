#!/usr/bin/env python3

import argparse
import os
from pathlib import Path

projectName = 'Blog.Api'

parser = argparse.ArgumentParser()

parser.add_argument('cmd')

args = parser.parse_args()

projectDir = Path(__file__).resolve().parent.parent
os.chdir(projectDir)

if args.cmd == 'generate-rsa':
    rsaPublicKeyFile = os.path.join(projectDir, 'rsa-public-key.pem')
    rsaSecretKeyFile = os.path.join(projectDir, 'rsa-private-key.pem')

    # generate private key
    os.system(f"openssl genrsa -out {rsaSecretKeyFile} 2048")
    # generate public key
    os.system(f"openssl rsa -in {rsaSecretKeyFile} -pubout -out {rsaPublicKeyFile}")

elif args.cmd == 'generate-ssl-cert':
    x509CertFile = os.path.join(projectDir, 'x509-cert.pem')
    x509CertKeyFile = os.path.join(projectDir, 'x509-cert-key.pem')

    # generate cert
    os.system(
        f"openssl req -x509 -newkey rsa:4096 -keyout {x509CertKeyFile} -out {x509CertFile} -sha256 -days 365 -nodes -subj /CN=localhost")
