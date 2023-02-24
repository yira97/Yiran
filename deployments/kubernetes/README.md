# prepare cli

macOS
```bash
# install kubectl
# https://kubernetes.io/docs/tasks/tools/install-kubectl-macos/
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/darwin/arm64/kubectl"

# install helm
# https://helm.sh/docs/intro/install/
brew install helm
```

finally
```bash
# make alias
alias alias k='kubectl --kubeconfig=/Users/yiran/yiran/backup/pw/vultr/kubernetes/configuration/vke-41db318e-558c-4f0e-9530-778987538e84.yaml'

k get nodes
```

# deploy manifests
```bash
# update the value of manifest
cp aws-key.secrets.yml.example aws-key.secrets.yml
cp evrane-blog.secrets.yml.example evrane-blog.secrets.yml

vim aws-key.secrets.yml
vim evrane-blog.secrets.yml

# deploy manifest
k apply -f aws-key.secrets.yml
k apply -f evrane-blog.cm.yml
k apply -f evrane-blog.secrets.yml
k apply -f evrane-blog.depl.yml
k apply -f evrane-blog.svc.yml

# install the cert-manager
k apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.11.0/cert-manager.yaml
# check the status of installation
k get pods --namespace cert-manager

vim letsencrypt-prod.issuer.yaml
vim letsencrypt-staging.issuer.yaml

k letsencrypt-prod.issuer.yaml
k letsencrypt-staging.issuer.yaml

vim nginx-ingress.ingress.yml

k apply -f nginx-ingress.ingress.yml
```