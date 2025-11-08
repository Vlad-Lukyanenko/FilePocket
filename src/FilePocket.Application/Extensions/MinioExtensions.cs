using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePocket.Application.Extensions
{
    public static class MinioExtensions
    {
        public static MinioObjectArgs GetObjectArgs(this FileMetadata metadata)
        {
            var firstSeparatorIndex = metadata.Path.IndexOf('/');

            if (firstSeparatorIndex <= 0)
                throw new ArgumentException($"Bucket not specified for file metadatd with id: {metadata.Id}");

            return new MinioObjectArgs()
            {
                BucketName = metadata.Path[..firstSeparatorIndex],
                ObjectName = string.Concat(metadata.Path[(firstSeparatorIndex + 1)..], '/', metadata.ActualName)
            };
        }
    }
}
