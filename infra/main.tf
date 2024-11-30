provider "aws" {
  region = "us-east-1"
}

# Caso o Terraform use backend no S3
terraform {
  backend "s3" {
    bucket = "terraform-tfstate-grupo12-fiap-2024-produto"
    key    = "lambda_produto/terraform.tfstate"
    region = "us-east-1"
  }
}

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
      },
    ]
  })
}

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

resource "aws_lambda_function" "produto_function" {
  function_name = "lambda_produto_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  # Codigo armazenado no S3
  s3_bucket = "code-lambdas-functions-produto"
  s3_key    = "lambda_produto_function.zip"
}

## Criação de lambdas somente para o API Gateway funcionar

resource "aws_lambda_function" "pedido_function" {
  function_name = "lambda_pedido_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  # Codigo armazenado no S3
  s3_bucket = "code-lambdas-functions-produto"
  s3_key    = "lambda_produto_function.zip"
}

resource "aws_lambda_function" "pagamento_function" {
  function_name = "lambda_pagamento_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  # Codigo armazenado no S3
  s3_bucket = "code-lambdas-functions-produto"
  s3_key    = "lambda_produto_function.zip"
}

resource "aws_lambda_function" "cliente_function" {
  function_name = "lambda_cliente_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  # Codigo armazenado no S3
  s3_bucket = "code-lambdas-functions-produto"
  s3_key    = "lambda_produto_function.zip"
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

# Criação do RDS SQL Server Express
resource "aws_db_instance" "sql_server" {
  allocated_storage    = 20
  storage_type         = "gp2"
  engine               = "sqlserver-ex"
  engine_version       = "15.00.4043.16.v1"  # Versão mais recente do SQL Server Express
  instance_class       = "db.t3.micro"
  db_name              = "ByteMeBurger"
  username             = "techchallenge"
  password             = "techchallenge"
  parameter_group_name = "default.sqlserver-ex-15.0"
  skip_final_snapshot  = true
  vpc_security_group_ids = [aws_security_group.rds_sg.id]
}

# Script inicial para criar banco de dados e tabelas
resource "local_file" "init_sql_script" {
  content = <<EOF
    CREATE TABLE Customers (
      CustomerID INT PRIMARY KEY,
      FirstName NVARCHAR(50),
      LastName NVARCHAR(50),
      Email NVARCHAR(100)
    );
  EOF
  filename = "init.sql"
}

# Execução do script inicial usando null_resource
resource "null_resource" "run_init_script" {
  provisioner "local-exec" {
    command = <<EOT
      sqlcmd -S ${aws_db_instance.sql_server.address},1433 \
      -U techchallenge \
      -P techchallenge \
      -i ${local_file.init_sql_script.filename}
    EOT
  }

  depends_on = [aws_db_instance.sql_server]
}

# Outputs para verificar a configuração
output "rds_endpoint" {
  value = aws_db_instance.sql_server.endpoint
}

output "db_username" {
  value = "techchallenge"
  sensitive = true
} 

output "db_password" {
  value     = "techchallenge"
  sensitive = true
}