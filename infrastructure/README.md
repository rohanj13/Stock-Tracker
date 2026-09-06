# Infrastructure as Code

This directory will eventually contain infrastructure configuration for AWS deployment.

## Planned Structure

- **terraform/** - Terraform IaC for AWS resources
  - lambda.tf - Lambda function configuration
  - api_gateway.tf - API Gateway setup
  - rds.tf - RDS PostgreSQL configuration (if not using Supabase)
  - iam.tf - IAM roles and policies
  - variables.tf - Input variables
  - outputs.tf - Output values

- **cloudformation/** - CloudFormation templates (alternative to Terraform)
  - template.yaml - Main template

- **docker/** - Docker configuration for local development and Lambda
  - Dockerfile - Container image for Lambda

## AWS Lambda Setup

The ASP.NET Core application is configured to run in AWS Lambda using:

- Amazon.Lambda.AspNetCoreServer.Hosting package
- API Gateway HTTP API as the entry point
- Direct ASP.NET Core minimal APIs without separate function handlers

## Database Configuration

For PostgreSQL hosting options:

- **Supabase**: PostgreSQL DBaaS with built-in auth
- **Amazon RDS**: Managed relational database
- **Amazon Aurora PostgreSQL**: Serverless option

## Current Status

Infrastructure configuration will be added incrementally as development progresses.
