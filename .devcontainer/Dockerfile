FROM mcr.microsoft.com/devcontainers/dotnet:10.0-noble

ENV http_proxy=http://proxy-n-wcg.marinha.pt:8080
ENV https_proxy=http://proxy-n-wcg.marinha.pt:8080
ENV no_proxy=".marinha.pt,localhost,127.0.0.1,.defesa.pt"

ENV Ldap__Marinha__Password=${Ldap__Marinha__Password}
ENV Keycloak__credentials__secret=${Keycloak__credentials__secret}
ENV ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection}
ENV Secrets__ProcessosPassword=${Ldap__Marinha__Password}

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    libldap2 libldap-common \
    && rm -rf /var/lib/apt/lists/*

COPY .devcontainer/certs/root-ca-ca.crt /usr/local/share/ca-certificates/
COPY .devcontainer/certs/marinha-root-ca.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates
