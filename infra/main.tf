provider "aws" {
  region = "us-east-1"
}

terraform {
  backend "s3" {
    bucket = "terraform-tfstate-grupo12-fiap-2024-produto"
    key    = "lambda_produto/terraform.tfstate"
    region = "us-east-1"
  }
}

# IAM Role para Lambda
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

# Política para Lambda
resource "aws_iam_policy" "lambda_policy" {
  name        = "lambda_produto_policy"
  description = "IAM policy for Lambda execution"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents",
          "dynamodb:DeleteItem",
          "dynamodb:GetItem",
          "dynamodb:PutItem",
          "dynamodb:Query",
          "dynamodb:Scan",
          "dynamodb:UpdateItem",
          "dynamodb:DescribeTable"
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

# RDS SQL Server
resource "aws_db_instance" "sql_server" {
  allocated_storage      = 20
  storage_type           = "gp2"
  engine                 = "sqlserver-ex"
  engine_version         = "15.00.4043.16.v1"
  instance_class         = "db.t3.micro"
  username               = "techchallenge"
  password               = "techchallenge"
  db_name                = "ByteMeBurger"
  parameter_group_name   = "default.sqlserver-ex-15.0"
  skip_final_snapshot    = true
  vpc_security_group_ids = [aws_security_group.rds_sg.id]
}

# Segurança: Grupo de Segurança do RDS
resource "aws_security_group" "rds_sg" {
  name_prefix = "rds-sg-"

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]  # Ajuste para maior segurança
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Lambda de Produto
resource "aws_lambda_function" "produto_function" {
  function_name = "lambda_produto_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto"
  s3_key        = "lambda_produto_function.zip"

  # Variáveis de ambiente
  environment {
    variables = {
      SQLServerConnection = "Server=${aws_db_instance.sql_server.address};Database=ByteMeBurger;User Id=techchallenge;Password=techchallenge;"
    }
  }
}

# Lambda de Pedido (Mantida conforme o original)
resource "aws_lambda_function" "pedido_function" {
  function_name = "lambda_pedido_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto"
  s3_key        = "lambda_produto_function.zip"
}

# Lambda de Pagamento (Mantida conforme o original)
resource "aws_lambda_function" "pagamento_function" {
  function_name = "lambda_pagamento_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto"
  s3_key        = "lambda_produto_function.zip"
}

# Lambda de Cliente (Mantida conforme o original)
resource "aws_lambda_function" "cliente_function" {
  function_name = "lambda_cliente_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto"
  s3_key        = "lambda_produto_function.zip"
}

# Outputs
output "rds_endpoint" {
  value = aws_db_instance.sql_server.endpoint
}

output "db_connection_string" {
  value = "Server=${aws_db_instance.sql_server.address};Database=ByteMeBurger;User Id=techchallenge;Password=techchallenge;"
}