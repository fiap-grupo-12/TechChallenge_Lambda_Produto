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
          "dynamodb:DescribeTable",
          "ec2:DescribeNetworkInterfaces",
          "ec2:CreateNetworkInterface",
          "ec2:DeleteNetworkInterface",
          "ec2:AttachNetworkInterface"
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

# VPC e Sub-rede
resource "aws_vpc" "main_vpc" {
  cidr_block = "10.0.0.0/16"
}

resource "aws_subnet" "lambda_subnet" {
  vpc_id                  = aws_vpc.main_vpc.id
  cidr_block              = "10.0.1.0/24"
  availability_zone       = "us-east-1a"
}

# Segurança: Grupo de Segurança do RDS
resource "aws_security_group" "rds_sg" {
  vpc_id = aws_vpc.main_vpc.id
  name_prefix = "rds-sg-"

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["10.0.1.0/24"] # Permite acesso a partir da sub-rede onde está a Lambda
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Segurança: Grupo de Segurança do Lambda
resource "aws_security_group" "lambda_sg" {
  vpc_id = aws_vpc.main_vpc.id
  name_prefix = "lambda-sg-"

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # Permitir que Lambda se conecte ao SQL Server no RDS
  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["10.0.0.0/16"] # Permite acesso ao RDS na VPC principal
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
  vpc_security_group_ids = [aws_security_group.rds_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.rds_subnet_group.name
}

# Grupo de Sub-redes para RDS
resource "aws_db_subnet_group" "rds_subnet_group" {
  name       = "rds_subnet_group"
  subnet_ids = [aws_subnet.lambda_subnet.id]

  tags = {
    Name = "RDS subnet group"
  }
}

# Lambda de Produto - Atualizado para usar a VPC
resource "aws_lambda_function" "produto_function" {
  function_name = "lambda_produto_function"
  role          = aws_iam_role.lambda_execution_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "FIAP.TechChallenge.LambdaProduto.API::FIAP.TechChallenge.LambdaProduto.API.Function::FunctionHandler"
  s3_bucket     = "code-lambdas-functions-produto2"
  s3_key        = "lambda_produto_function.zip"

  # Variáveis de ambiente
  environment {
    variables = {
      SQLServerConnection = "Server=${aws_db_instance.sql_server.address};Database=ByteMeBurger;User Id=techchallenge;Password=techchallenge;"
    }
  }

  # Configuração de VPC para Lambda
  vpc_config {
    subnet_ids         = [aws_subnet.lambda_subnet.id]
    security_group_ids = [aws_security_group.lambda_sg.id]
  }
}

# Outputs
output "rds_endpoint" {
  value = aws_db_instance.sql_server.endpoint
}

output "db_connection_string" {
  value = "Server=${aws_db_instance.sql_server.address};Database=ByteMeBurger;User Id=techchallenge;Password=techchallenge;"
}