FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base


ENV TZ 'Europe/Kiev'
    RUN echo $TZ > /etc/timezone && \
    apt-get update && apt-get install -y tzdata && \
    rm /etc/localtime && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    apt-get clean

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_16.x | bash \
    && apt-get install nodejs -yq \
	&& curl -L https://www.npmjs.com/install.sh

RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf

EXPOSE 8080
COPY . .
ENTRYPOINT ["dotnet", "Organization.Api.dll"]
