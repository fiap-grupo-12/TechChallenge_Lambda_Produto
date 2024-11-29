provider "aws" {
  region = "us-east-1"
}

#Caso o 
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

resource "aws_lambda_function" "pedido_function" {
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

#Criacao do SQL Server RDS

