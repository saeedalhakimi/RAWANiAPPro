using Microsoft.Extensions.Diagnostics.HealthChecks;
using RAWANi.WEBAPi.Application.Repository;

namespace RAWANi.WEBAPi.Health
{
    public class PostRepsitoryHealthCheck : IHealthCheck
    {
        private readonly IPostRepository _postRepository;

        public PostRepsitoryHealthCheck(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var hasPosts = await _postRepository.PostsHealthCheckAsync(cancellationToken);
                return hasPosts
                   ? HealthCheckResult.Healthy("Posts table is accessible and has data.")
                   : HealthCheckResult.Degraded("Posts table is accessible but empty.");

            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Failed to check Posts table health.", ex);
            }
        }
    }
}
