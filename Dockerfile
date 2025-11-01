# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ENV http_proxy=http://proxy-n-wcg.marinha.pt:8080
ENV https_proxy=http://proxy-n-wcg.marinha.pt:8080
ENV no_proxy="marinha.pt,.marinha.pt,localhost"
ENV HUSKY=0

WORKDIR /app
COPY ./Processos-Juridicos ./

RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

ENV http_proxy=http://proxy-n-wcg.marinha.pt:8080
ENV https_proxy=http://proxy-n-wcg.marinha.pt:8080
ENV no_proxy="marinha.pt,.marinha.pt,localhost"

RUN apt-get update \
    && apt-get install -y \
    libldap-2.5-0 && rm -rf /var/lib/apt/lists/*

COPY certs/marinha-root-ca.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
CMD ["dotnet", "Processos-Juridicos.dll"]