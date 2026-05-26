# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG HTTP_PROXY_ARG
ARG HTTPS_PROXY_ARG
ARG NO_PROXY_ARG

ENV http_proxy="${HTTP_PROXY_ARG}"
ENV https_proxy="${HTTPS_PROXY_ARG}"
ENV no_proxy="${NO_PROXY_ARG}"
ENV HUSKY=0

WORKDIR /app
COPY ./Processos-Juridicos ./
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

ARG HTTP_PROXY_ARG
ARG HTTPS_PROXY_ARG
ARG NO_PROXY_ARG

ENV http_proxy="${HTTP_PROXY_ARG}"
ENV https_proxy="${HTTPS_PROXY_ARG}"
ENV no_proxy="${NO_PROXY_ARG}"

RUN apt-get update \ 
    && apt-get install -y --no-install-recommends \ 
    libldap2 libldap-common \ 
    && rm -rf /var/lib/apt/lists/*

COPY certs/root-ca-ca.crt /usr/local/share/ca-certificates/
COPY certs/marinha-root-ca.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
CMD ["dotnet", "Processos-Juridicos.dll"]