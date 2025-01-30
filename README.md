# ML-Agents Self-Driving Car

This project uses Unity's ML-Agents Toolkit to train a car to autonomously navigate a race track using reinforcement learning.

## Features
- Deep reinforcement learning-based self-driving car.
- The car learns to follow the track and avoid obstacles.
- Multiple training rewards for speed, staying on track, and completing laps.

## Requirements
- Unity ML-Agents Toolkit (`ml-agents`)
- Python 3.8+
- TensorFlow (for training the model)

## Training
The agent receives rewards for:
- Staying on the track.
- Moving at an optimal speed.
- Completing laps efficiently.

## PPO Algorithm
Proximal Policy Optimization (PPO) is the reinforcement learning algorithm used to train the self-driving car. It improves upon earlier methods like Trust Region Policy Optimization (TRPO) by balancing sample efficiency and stability.

### How PPO Works
1. **Policy-Based Learning**  
   - PPO learns a policy \( \pi_\theta(a | s) \), which determines the probability of taking action \( a \) given state \( s \).
   - Instead of directly estimating Q-values like DQN, PPO optimizes the policy function.

2. **Clipped Objective Function**  
   - PPO uses a clipping mechanism to limit how much the policy changes per update.
   - The objective function ensures that updates do not cause drastic shifts in behavior.
   - This prevents the model from diverging and leads to more stable learning.

3. **Advantage Estimation**  
   - Uses Generalized Advantage Estimation (GAE) to reduce variance and improve learning efficiency.
   - The agent learns from rewards and estimates how much better (or worse) an action is compared to the average expectation.

## Usage
Once training is complete, the trained model can be loaded into the Unity environment to control the car autonomously.

## License
This project is open-source and free to use.
