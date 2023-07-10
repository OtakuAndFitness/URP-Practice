#ifndef VORONOI_NOISE
#define VORONOI_NOISE

#include "WhiteNoise.hlsl"

float2 voronoi_noise_randomVector (float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
}

float2 Voronoi(float2 UV, float AngleOffset, float CellDensity)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);
    float MinDistanceToCell;
    float Cell;
    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = voronoi_noise_randomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);
            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                MinDistanceToCell = res.x;
                Cell = res.y;
            }
        }
    }
    return float2(MinDistanceToCell, Cell);
}

float sdStar5(float2 p, float r, float rf) //sdStar5(i.uv - 0.5, 0.25, 0.5);
{
    const float2 k1 = float2(0.809016994375, -0.587785252292);
    const float2 k2 = float2(-k1.x, k1.y);
    p.x = abs(p.x);
    p -= 2.0 * max(dot(k1, p), 0.0) * k1;
    p -= 2.0 * max(dot(k2, p), 0.0) * k2;
    p.x = abs(p.x);
    p.y -= r;
    float2 ba = rf * float2(-k1.y, k1.x) - float2(0, 1);
    float h = clamp(dot(p, ba)/dot(ba, ba), 0.0, r);
    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}

float2 StarNoise(float2 UV, float AngleOffset, float CellDensity, float r, float rf) //SDF Noise
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);
    float MinDistanceToCell;
    float Cell;
    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = voronoi_noise_randomVector(lattice + g, AngleOffset);
            float d = sdStar5(lattice + offset - f, r, rf);
            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                MinDistanceToCell = res.x;
                Cell = res.y;
            }
        }
    }
    return float2(MinDistanceToCell, Cell);
}

float3 voronoiNoise2D(float2 value)
{
	float2 baseCell = floor(value);

	//first pass to find the closest cell
	float minDistToCell = 10;
	float2 toClosestCell;
	float2 closestCell;
	[unroll]
	for(int x1=-1; x1<=1; x1++){
		[unroll]
		for(int y1=-1; y1<=1; y1++){
			float2 cell = baseCell + float2(x1, y1);
			float2 cellPosition = cell + rand2dTo2d(cell);
			float2 toCell = cellPosition - value;
			float distToCell = length(toCell);
			if(distToCell < minDistToCell){
				minDistToCell = distToCell;
				closestCell = cell;
				toClosestCell = toCell;
			}
		}
	}

	//second pass to find the distance to the closest edge
	float minEdgeDistance = 10;
	[unroll]
	for(int x2=-1; x2<=1; x2++){
		[unroll]
		for(int y2=-1; y2<=1; y2++){
			float2 cell = baseCell + float2(x2, y2);
			float2 cellPosition = cell + rand2dTo2d(cell);
			float2 toCell = cellPosition - value;

			float2 diffToClosestCell = abs(closestCell - cell);
			bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y < 0.1;
			if(!isClosestCell){
				float2 toCenter = (toClosestCell + toCell) * 0.5;
				float2 cellDifference = normalize(toCell - toClosestCell);
				float edgeDistance = dot(toCenter, cellDifference);
				minEdgeDistance = min(minEdgeDistance, edgeDistance);
			}
		}
	}

	float random = rand2dTo1d(closestCell);
	return float3(minDistToCell, random, minEdgeDistance);
}

float3 voronoiNoise3D(float3 value)
{
	float3 baseCell = floor(value);

	//first pass to find the closest cell
	float minDistToCell = 10;
	float3 toClosestCell;
	float3 closestCell;
	[unroll]
	for(int x1=-1; x1<=1; x1++){
		[unroll]
		for(int y1=-1; y1<=1; y1++){
			[unroll]
			for(int z1=-1; z1<=1; z1++){
				float3 cell = baseCell + float3(x1, y1, z1);
				float3 cellPosition = cell + rand3dTo3d(cell);
				float3 toCell = cellPosition - value;
				float distToCell = length(toCell);
				if(distToCell < minDistToCell){
					minDistToCell = distToCell;
					closestCell = cell;
					toClosestCell = toCell;
				}
			}
		}
	}

	//second pass to find the distance to the closest edge
	float minEdgeDistance = 10;
	[unroll]
	for(int x2=-1; x2<=1; x2++){
		[unroll]
		for(int y2=-1; y2<=1; y2++){
			[unroll]
			for(int z2=-1; z2<=1; z2++){
				float3 cell = baseCell + float3(x2, y2, z2);
				float3 cellPosition = cell + rand3dTo3d(cell);
				float3 toCell = cellPosition - value;

				float3 diffToClosestCell = abs(closestCell - cell);
				bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
				if(!isClosestCell){
					float3 toCenter = (toClosestCell + toCell) * 0.5;
					float3 cellDifference = normalize(toCell - toClosestCell);
					float edgeDistance = dot(toCenter, cellDifference);
					minEdgeDistance = min(minEdgeDistance, edgeDistance);
				}
			}
		}
	}

	float random = rand3dTo1d(closestCell);
	return float3(minDistToCell, random, minEdgeDistance);
}

#endif