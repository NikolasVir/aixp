# Step 1: Clone from GitHub 
FROM alpine/git AS source
RUN git clone --branch main --single-branch \
    https://github.com/NikolasVir/aixp.git /src

# Step 2: Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-api
WORKDIR /src
COPY --from=source /src/AIXP.API .
RUN dotnet publish -c Release -o /out/api

# Step 3: Build Angular UI 
FROM node:20 AS build-ui
WORKDIR /ui
COPY --from=source /src/AIXP.UI .
RUN npm install && npm run build -- --configuration production

# Final: API image 
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
COPY --from=build-api /out/api .
ENTRYPOINT ["dotnet", "AIXP.API.dll"]

#  Final: UI image (nginx) 
FROM nginx:alpine AS ui
COPY --from=build-ui /ui/dist/AIXP.UI/browser /usr/share/nginx/html
