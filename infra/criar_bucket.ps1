# Nome do bucket e região
$bucketName = "terraform-tfstate-grupo12-fiap-2024-produto3"
$region = "us-east-1"

# Criar o bucket (sem LocationConstraint para us-east-1, pois não é necessário)
Write-Host "Criando bucket S3: $bucketName na região $region..."
aws s3api create-bucket --bucket $bucketName --region $region

# Habilitar versionamento no bucket
Write-Host "Habilitando versionamento no bucket: $bucketName..."
aws s3api put-bucket-versioning --bucket $bucketName --versioning-configuration Status=Enabled

# Aplicar política para bloquear acessos públicos
Write-Host "Bloqueando acessos públicos ao bucket: $bucketName..."
aws s3api put-public-access-block --bucket $bucketName --public-access-block-configuration `
    "{\"BlockPublicAcls\":true,\"IgnorePublicAcls\":true,\"BlockPublicPolicy\":true,\"RestrictPublicBuckets\":true}"

# Configurar criptografia padrão para o bucket
Write-Host "Configurando criptografia para o bucket: $bucketName..."
aws s3api put-bucket-encryption --bucket $bucketName --server-side-encryption-configuration `
    "{\"Rules\":[{\"ApplyServerSideEncryptionByDefault\":{\"SSEAlgorithm\":\"AES256\"}}]}"

# Mensagem de confirmação
Write-Host "Bucket $bucketName criado e configurado com sucesso na região $region."