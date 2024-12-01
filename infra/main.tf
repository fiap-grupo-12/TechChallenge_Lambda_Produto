provider "aws" {
  region = "us-east-1"
}

terraform {
  backend "s3" {
    bucket = "terraform-tfstate-grupo12-fiap-2024-produto2"
    key    = "lambda_produto/terraform.tfstate"
    region = "us-east-1"
  }
}

# IAM Role for Lambda
resource "aws_iam_role" "lambda_execution_role" {
  name = "lambda_produto_execution_role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

# Policy for Lambda
resource "aws_iam_policy" "lambda_policy" {
  name        = "lambda_produto_policy"
  description = "IAM policy for Lambda execution"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "ec2:DescribeNetworkInterfaces",
          "ec2:CreateNetworkInterface",
          "ec2:DeleteNetworkInterface",
          "ec2:AttachNetworkInterface",
          "secretsmanager:GetSecretValue",
          "rds:*",
          "logs:*",
          "cloudwatch:*",
        ]
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_execution_policy" {
  role       = aws_iam_role.lambda_execution_role.name
  policy_arn = aws_iam_policy.lambda_policy.arn
}

# variables.tf
variable "vpc_id" {
  description = "The ID of the VPC"
  type        = string
}

# main.tf
provider "aws" {
  region = "us-west-2"
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [var.vpc_id]
  }
}

output "subnet_ids" {
  value = data.aws_subnets.default.ids
}

# Use Default VPC and Subnets
data "aws_vpc" "default" {
  default = true
}

data "aws_subnet_ids" "default" {
  vpc_id = data.aws_vpc.default.id
}

# Security Group for RDS and Lambda
resource "aws_security_group" "common_sg" {
  vpc_id = data.aws_vpc.default.id
  name   = "common-sg"

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"] # Allow access to SQL Server from anywhere
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# RDS SQL Server
resource "aws_db_instance" "sql_server" {
  allocated_storage      = 20
  storage_type           = "gp2"
  engine                 = "sqlserver-ex"
  engine_version         = "15.00.4043.16.v1"
  instance_class         = "db.t3.micro"
  username               = "techchallenge"
  password               = "techchallenge"
  parameter_group_name   = "default.sqlserver-ex-15.0"
  skip_final_snapshot    = true
  vpc_security_group_ids = [aws_security_group.common_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.rds_subnet_group.name
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [var.vpc_id]  # Certifique-se de definir a variável vpc_id corretamente
  }
}

output "subnet_ids" {
  value = data.aws_subnets.default.ids
}

# Lambda Function
resource "aws_lambda_function" "produto_function" {
  function_name = "lambda_produto_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto2"
  s3_key        = "lambda_produto_function.zip"

  # Environment Variables
  environment {
    variables = {
      SQLServerConnection = "Server=${aws_db_instance.sql_server.address};Database=ByteMeBurger;User Id=techchallenge;Password=techchallenge;"
    }
  }

  resource "aws_lambda_function" "pedido_function" {
  function_name = "lambda_pedido_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto2"
  s3_key        = "lambda_produto_function.zip"
}

resource "aws_lambda_function" "cliente_function" {
  function_name = "lambda_cliente_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto2"
  s3_key        = "lambda_produto_function.zip"
}

resource "aws_lambda_function" "pagamento_function" {
  function_name = "lambda_pagamento_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto2"
  s3_key        = "lambda_produto_function.zip"
}

vpc_config {
    subnet_ids         = data.aws_subnet_ids.default.ids
    security_group_ids = [aws_security_group.common_sg.id]
  }
}

# Outputs
output "rds_endpoint" {
  value = aws_db_instance.sql_server.endpoint
}

output "db_connection_string" {
  value = "Server=${aws_db_instance.sql_server.address};Database=ByteMeBurger;User Id=techchallenge;Password=techchallenge;"
}