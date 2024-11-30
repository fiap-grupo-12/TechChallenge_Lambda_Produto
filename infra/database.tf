provider "aws" {
  region = "us-east-1"
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